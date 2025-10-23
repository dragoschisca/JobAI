import os
import PyPDF2
import google.generativeai as genai

genai.configure(api_key=os.environ.get("GEMINI_API_KEY"))

def extract_text(pdf_file):
    with open(pdf_file, "rb") as file:
        reader = PyPDF2.PdfReader(file)
        text = ""
        for page in reader.pages:
            text += page.extract_text()
        return text

def send_text_to_gemini(file, description, skills):

    text = extract_text(file)

    model = genai.GenerativeModel("gemini-2.5-flash")
    response = model.generate_content(f"Esti un HR din IT cu experienta de 20 ani, analizeaza cv si apreciazal de la 1 la 100 "
                                      f"pentru postul de Developer Backend сe are descrierea ${description} si tehnologiile necesare: "
                                      f"${skills} "
                                      f"returneaza doar valoarea de la 1 la 100 care va reprezenta potriverea pentru locul de munca ${text}")

    print(response.text)

