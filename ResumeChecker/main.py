import os
import sys
import json
import PyPDF2
from google import genai
from pydantic import WithJsonSchema

client = genai.Client(api_key="AIzaSyBMmL3Z8OoSH1DCVZFgoE2pIMqKdx1Wdp0")

data = json.loads(sys.stdin.read())

def json_to_variable():
    cv_path = data["cvPath"]
    job_description = data["jobDescription"]
    job_skills = data["jobSkills"]

    return cv_path, job_description, job_skills

def extract_text(pdf_file):
    with open(pdf_file, "rb") as file:
        reader = PyPDF2.PdfReader(file)
        text = ""
        for page in reader.pages:
            text += page.extract_text()
        return text

def send_text_to_gemini():

    cvPath, jobDescription, jobSkills = json_to_variable()

    text = extract_text(cvPath)

    response_stream = client.models.generate_content_stream(
        model="gemini-2.0-flash",
        contents=f"Esti un HR din IT cu experienta de 20 ani, analizeaza cv si apreciazal de la 1 la 100 "
                 f"pentru postul de Developer Backend ce are descrierea {jobDescription} si tehnologiile necesare: "
                 f"{jobSkills} "
                 f"returneaza STRICT DOAR valoarea de la 1 la 100 care va reprezenta potrivirea pentru locul de munca {text}"
    )

    # adunăm toate bucățile generate într-un singur string
    final_text = ""
    for chunk in response_stream:
        final_text += chunk.text  # fiecare chunk are atributul .text

    final_text = final_text.replace("\n", "")

    result = {
        "score" : final_text
    }

    print(json.dumps(result))

    return json.dumps(result)


send_text_to_gemini()