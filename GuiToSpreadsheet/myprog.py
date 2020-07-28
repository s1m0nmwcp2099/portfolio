from openpyxl import *
from tkinter import *

#globally declare wb and sheet variable

#open the existing excel file
wb = load_workbook("spreadsheet.xlsx")

#create the sheet object
sheet = wb.active
def excel():
    #resize the width columns in excel spreadsheet
    sheet.column_dimensions['A'].width = 30
    sheet.column_dimensions['B'].width = 10
    sheet.column_dimensions['C'].width = 10
    sheet.column_dimensions['D'].width = 20
    sheet.column_dimensions['E'].width = 20
    sheet.column_dimensions['F'].width = 40
    sheet.column_dimensions['G'].width = 50

    #write given data to spreadsheet at given locn
    sheet.cell(row = 1, column = 1).value = "Name"
    sheet.cell(row = 1, column = 2).value = "Course"
    sheet.cell(row = 1, column = 3).value = "Semester"
    sheet.cell(row = 1, column = 4).value = "Form Number"
    sheet.cell(row = 1, column = 5).value = "Contact Number"
    sheet.cell(row = 1, column = 6).value = "Email id"
    sheet.cell(row = 1, column = 7).value = "Address"

#function to set focus (cursor)
def focus1(event):
    #set focus on course_field box
    course_field.focus_set()