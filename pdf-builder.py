import os
import tkinter as tk
from tkinter import filedialog, messagebox
from pypdf import PdfWriter
from PIL import Image
from reportlab.platypus import SimpleDocTemplate, Paragraph, Preformatted, Spacer
from reportlab.lib.styles import getSampleStyleSheet
from reportlab.lib.pagesizes import A4

def ordner_auswaehlen():
    ordner_pfad = filedialog.askdirectory(title="Hauptordner auswählen")
    if ordner_pfad:
        eingabe_ordner_var.set(ordner_pfad)

def speicherort_auswaehlen():
    datei_pfad = filedialog.asksaveasfilename(
        title="Speicherort für das neue PDF",
        defaultextension=".pdf",
        filetypes=[("PDF Dateien", "*.pdf")]
    )
    if datei_pfad:
        ausgabe_datei_var.set(datei_pfad)

def text_zu_pdf_umwandeln(datei_pfad, ausgabe_pdf_pfad):
    """Liest Text/Code-Dateien und generiert ein PDF. Fängt Binärdateien ab."""
    doc = SimpleDocTemplate(ausgabe_pdf_pfad, pagesize=A4)
    styles = getSampleStyleSheet()
    style_code = styles["Code"] # Monospace-Schriftart, perfekt für .cs Code
    style_titel = styles["Heading2"]
    
    story = []
    titel = f"Datei: {os.path.basename(datei_pfad)}"
    story.append(Paragraph(f"<b>{titel}</b>", style_titel))
    story.append(Spacer(1, 10))
    
    ist_text = True
    text_inhalt = ""
    
    # Versuch, die Datei als Text zu lesen
    try:
        with open(datei_pfad, 'r', encoding='utf-8') as f:
            text_inhalt = f.read()
    except UnicodeDecodeError:
        try:
            # Fallback für andere Codierungen
            with open(datei_pfad, 'r', encoding='latin-1') as f:
                text_inhalt = f.read()
        except Exception:
            ist_text = False
    except Exception:
        ist_text = False
        
    # Heuristik: Wenn Null-Bytes vorkommen, ist es definitiv eine Binärdatei
    if ist_text and '\0' in text_inhalt:
        ist_text = False

    if not ist_text:
        text_inhalt = "<< Diese Datei ist binär (z.B. .exe, .zip) oder ein proprietäres Format (z.B. .docx). Sie kann nicht direkt als Text gelesen werden. >>"
        style_code = styles["Normal"]
    else:
        # HTML/XML Sonderzeichen escapen, damit Reportlab nicht abstürzt
        text_inhalt = text_inhalt.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;')
    
    # Preformatted behält Einrückungen und Zeilenumbrüche exakt bei
    story.append(Preformatted(text_inhalt, style_code))
    
    try:
        doc.build(story)
        return True
    except Exception:
        return False

def pdf_erstellen():
    eingabe_ordner = eingabe_ordner_var.get()
    ausgabe_datei = ausgabe_datei_var.get()

    if not eingabe_ordner or not ausgabe_datei:
        messagebox.showwarning("Fehler", "Bitte wähle zuerst einen Ordner und einen Speicherort aus.")
        return

    merger = PdfWriter()
    temp_dateien = []
    unterstuetzte_bilder = ['.jpg', '.jpeg', '.png']
    dateien_gefunden = False

    try:
        status_var.set("Verarbeite Dateien... Bitte warten.")
        root.update()

        # Ordner und alle Unterordner durchsuchen
        for aktueller_ordner, _, dateien in os.walk(eingabe_ordner):
            for datei in sorted(dateien): # Alphabetisch sortieren
                datei_pfad = os.path.join(aktueller_ordner, datei)
                endung = os.path.splitext(datei)[1].lower()

                # Temporären Pfad für generierte PDFs erstellen
                temp_pdf_pfad = datei_pfad + "_temp.pdf"

                if endung == '.pdf':
                    # Bereits ein PDF, direkt anhängen
                    merger.append(datei_pfad)
                    dateien_gefunden = True

                elif endung in unterstuetzte_bilder:
                    # Bilder konvertieren
                    try:
                        bild = Image.open(datei_pfad)
                        pdf_bytes = bild.convert('RGB')
                        pdf_bytes.save(temp_pdf_pfad)
                        merger.append(temp_pdf_pfad)
                        temp_dateien.append(temp_pdf_pfad)
                        dateien_gefunden = True
                    except Exception as e:
                        print(f"Fehler beim Bild {datei_pfad}: {e}")

                else:
                    # Alle anderen Dateien (wie .cs, .txt, .js oder unbekannt) als Text-PDF generieren
                    if text_zu_pdf_umwandeln(datei_pfad, temp_pdf_pfad):
                        merger.append(temp_pdf_pfad)
                        temp_dateien.append(temp_pdf_pfad)
                        dateien_gefunden = True

        if not dateien_gefunden:
            messagebox.showinfo("Info", "Der Ordner ist komplett leer.")
            status_var.set("Bereit")
            return

        # Finale PDF speichern
        merger.write(ausgabe_datei)
        merger.close()

        # Temporäre Dateien restlos löschen
        for temp_datei in temp_dateien:
            if os.path.exists(temp_datei):
                os.remove(temp_datei)

        status_var.set("Erfolgreich abgeschlossen!")
        messagebox.showinfo("Erfolg", f"Das PDF wurde erfolgreich erstellt:\n{ausgabe_datei}")

    except Exception as e:
        status_var.set("Ein Fehler ist aufgetreten.")
        messagebox.showerror("Fehler", f"Fehler beim Erstellen des PDFs:\n{str(e)}")

# --- GUI Setup ---
root = tk.Tk()
root.title("Universal Folder to PDF Converter")
root.geometry("500x250")
root.resizable(False, False)

eingabe_ordner_var = tk.StringVar()
ausgabe_datei_var = tk.StringVar()
status_var = tk.StringVar()
status_var.set("Bereit")

# UI Elemente
tk.Label(root, text="1. Ordner (inkl. Unterordner) auswählen:").pack(pady=(10, 0))
tk.Entry(root, textvariable=eingabe_ordner_var, width=50, state="readonly").pack()
tk.Button(root, text="Ordner durchsuchen", command=ordner_auswaehlen).pack(pady=5)

tk.Label(root, text="2. Speicherort für das fertige PDF wählen:").pack(pady=(10, 0))
tk.Entry(root, textvariable=ausgabe_datei_var, width=50, state="readonly").pack()
tk.Button(root, text="Speicherort wählen", command=speicherort_auswaehlen).pack(pady=5)

tk.Button(root, text="PDF jetzt erstellen", command=pdf_erstellen, bg="#007BFF", fg="white", font=("Arial", 10, "bold")).pack(pady=15)

tk.Label(root, textvariable=status_var, fg="blue").pack()

root.mainloop()