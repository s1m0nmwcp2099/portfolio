from tkinter import *
from tkinter import ttk
import tkinter as tk
from fpdf import FPDF
pdf = FPDF()
import pandas as pd
import numpy as np
from tkcalendar import Calendar, DateEntry
import datetime
from functools import partial


# date
def setDate(x):
    def define_dates():
        if x == 'Set invoice date':
            inv_date = cal.selection_get()
            print(f'Invoice date: {inv_date}')
            inv_dt_show.config(text=str(inv_date))
            # inv_dt_show.insert(INSERT, str(inv_date))
        elif x == 'Set due date':
            due_date = cal.selection_get()
            print(f'Due by {due_date}')
            due_dt_show.config(text=str(due_date))
            # due_dt_show.insert(INSERT, str(due_date))
        cal.see(datetime.date(year=2016, month=2, day=5))
        top.destroy()

    top = tk.Toplevel(root)

    # today = datetime.date.today()
    today = datetime.date.today()

    mindate = today - datetime.timedelta(days=365)
    maxdate = today + datetime.timedelta(days=365)
    print(mindate, maxdate)


    cal = Calendar(top, font="Arial 14", selectmode='day', locale='en_US',
            mindate=mindate, maxdate=maxdate, disabledforeground='red',
            cursor="hand1")
    cal.pack(fill="both", expand=True)
    ttk.Button(top, text=x, command=define_dates).pack()


def delete_action(i):
    # i = delete_button.grid_info()['row']-5
    desc_items[i].destroy()
    quantity_items[i].destroy()
    price_items[i].destroy()
    amount_items[i].destroy()
    tax_percents[i].destroy()
    edit_btns[i].destroy()
    delete_btns[i].destroy()
    
    is_active[i] = False

"""
def add_item_to_main_gui(_description, _quantity, _price, _amount, _t_mult, i):
    price_string = '£'+('%.2f' %_price)
    amount_string = '£'+('%.2f' %_amount)

    if _t_mult == 0.0:
        tax_rate_str = '0%'
    elif _t_mult == 0.2:
        tax_rate_str = '20%'
    elif _t_mult == 0.05:
        tax_rate_str = '5%'

    if i < len(desc_items):
        # desc_items[i] = Text(invGuiFrame, width=40, height=3)
        desc_items[i].insert('0.0', _description)
        desc_items[i].configure(status=DISABLED)
        descs[i] = _description


        # quantity_items[i] = Label(invGuiFrame, text=_quantity)
        quantity_items[i].configure(text=_quantity)
        quantities[i] = _quantity

        # price_items[i] = Label(invGuiFrame, text=price_string)
        price_items[i].configure(text=price_string)
        prices.append[i] = _price

        # amount_items.append[i] = Label(invGuiFrame, text=amount_string)
        amount_items[i].configure(text=amount_string)
        amounts.append[i] = _amount

        # tax_percents[i] = Label(invGuiFrame, text=tax_rate_str)
        tax_percents[i].configure(text=tax_rate_str)
        tax_rate_strs[i] = tax_rate_str
        tax_mults[i] = _t_mult
    else:
        desc_items.append(Text(invGuiFrame, width=40, height=3))
        #desc_items.append(Label(invGuiFrame, width=40, text=_description))
        descs.append(_description)
        desc_items[len(desc_items)-1].grid(row=len(desc_items)+4, column=0)
        desc_items[len(desc_items)-1].insert('0.0', _description)
        desc_items[len(desc_items)-1].configure(state=DISABLED)

        quantity_items.append(Label(invGuiFrame, text=_quantity))
        quantities.append(_quantity)
        quantity_items[len(quantity_items)-1].grid(row=len(quantity_items)+4, column=1)

        price_items.append(Label(invGuiFrame, text=price_string))
        prices.append(_price)
        price_items[len(price_items)-1].grid(row=len(price_items)+4, column=2)

        amount_items.append(Label(invGuiFrame, text=amount_string))
        amounts.append(_amount)
        amount_items[len(amount_items)-1].grid(row=len(amount_items)+4, column=3)

        tax_percents.append(Label(invGuiFrame, text=tax_rate_str))
        tax_rate_strs.append(tax_rate_str)
        tax_percents[len(tax_percents)-1].grid(row=len(tax_percents)+4, column=4)

        edit_button = Button(invGuiFrame, text='Edit')
        edit_button.grid(row=len(edit_btns)+5, column=5)
        edit_button.configure(command=partial(edit_item, edit_button.grid_info()['row']-5))
        edit_btns.append(edit_button)

        delete_button = Button(invGuiFrame, text='Delete')
        delete_button.grid(row=len(delete_btns)+5, column=6)
        delete_button.configure(command=partial(delete_action, delete_button.grid_info()['row']-5))
        delete_btns.append(delete_button)

        is_active.append(True)"""


def edit_item(i):
    print(f'Edit button {i} pressed')
    item_creator(False, i)


# def item_creator(description, quant, price):
def item_creator(is_new, i):
    if is_new == True:
        description = ''
        quant = 1
        price = 0.00
    else:
        description = descs[i]
        quant = quantities[i]
        price = prices[i]
        
    # add item button
    def add_item():
        def add_item_to_main_gui(i):
            price_string = '£'+('%.2f' %price)
            amount_string = '£'+('%.2f' %amount)

            if vatVar.get() == vat_options[0]:
                tax_rate_str = '0%'
                tax_mult = 0.0
            elif vatVar.get() == vat_options[1]:
                tax_rate_str = '20%'
                tax_mult = 0.2
            elif vatVar.get() == vat_options[2]:
                tax_rate_str = '5%'
                tax_mult = 0.05

            if is_new == False: # for editing an existing item
                descs[i] = description
                desc_items[i].destroy()
                desc_items[i] = Text(invGuiFrame, width=40, height=3)
                desc_items[i].grid(row=5+i, column=0)
                desc_items[i].insert('0.0', description)
                desc_items[i].configure(state=DISABLED)

                quantities[i] = quantity
                quantity_items[i].destroy()
                quantity_items[i] = Label(invGuiFrame, text=quantity)
                quantity_items[i].grid(row=5+i, column=1)

                prices[i] = price
                price_items[i].destroy()
                price_items[i] = Label(invGuiFrame, text=price_string)
                price_items[i].grid(row=5+i, column=2)
                
                amounts[i] = amount
                amount_items[i].destroy()
                amount_items[i] = Label(invGuiFrame, text=amount_string)
                amount_items[i].grid(row=5+i, column=3)

                tax_rate_strs[i] = vatVar.get()
                tax_percents[i].destroy()
                tax_percents[i] = Label(invGuiFrame, text=vatVar.get())
                tax_percents[i].grid(row=5+i, column=4)
                tax_mults[i] = tax_mult
            else:
                desc_items.append(Text(invGuiFrame, width=40, height=3))
                #desc_items.append(Label(invGuiFrame, width=40, text=description))
                descs.append(description)
                desc_items[len(desc_items)-1].grid(row=len(desc_items)+4, column=0)
                desc_items[len(desc_items)-1].insert('0.0', description)
                desc_items[len(desc_items)-1].configure(state=DISABLED)

                quantity_items.append(Label(invGuiFrame, text=quantity))
                quantities.append(quantity)
                quantity_items[len(quantity_items)-1].grid(row=len(quantity_items)+4, column=1)

                price_items.append(Label(invGuiFrame, text=price_string))
                prices.append(price)
                price_items[len(price_items)-1].grid(row=len(price_items)+4, column=2)

                amount_items.append(Label(invGuiFrame, text=amount_string))
                amounts.append(amount)
                amount_items[len(amount_items)-1].grid(row=len(amount_items)+4, column=3)

                tax_percents.append(Label(invGuiFrame, text=vatVar.get()))
                tax_rate_strs.append(tax_rate_str)
                tax_mults.append(tax_mult)
                tax_percents[len(tax_percents)-1].grid(row=len(tax_percents)+4, column=4)

                edit_button = Button(invGuiFrame, text='Edit')
                edit_button.grid(row=len(edit_btns)+5, column=5)
                edit_button.configure(command=partial(edit_item, edit_button.grid_info()['row']-5))
                edit_btns.append(edit_button)

                delete_button = Button(invGuiFrame, text='Delete')
                delete_button.grid(row=len(delete_btns)+5, column=6)
                delete_button.configure(command=partial(delete_action, delete_button.grid_info()['row']-5))
                delete_btns.append(delete_button)

                is_active.append(True)
        # headers
        if len(desc_items) == 0 and is_new == True:
            Label(invGuiFrame, text='Items').grid(row=4, column=0)
            Label(invGuiFrame, text='Quantity').grid(row=4, column=1)
            Label(invGuiFrame, text='Price').grid(row=4, column=2)
            Label(invGuiFrame, text='Amount').grid(row=4, column=3)
            Label(invGuiFrame, text='Tax').grid(row=4, column=4)
        
        description = description_box.get(1.0, "end-1c")
        quantity = int(quantity_box.get())
        price = float(price_box.get())
        amount = quantity*price
        if is_new == True:
            x = len(desc_items)
        else:
            x = i
        add_item_to_main_gui(x)
        subframe.destroy()

    subframe = tk.Toplevel(root)
    subframe.grid()
    subframe.columnconfigure(0, weight=1)
    subframe.rowconfigure(0, weight=1)
    
    # create job description
    description_box = Text(subframe, width=40, height=3)
    description_box.insert('0.0', description)
    Label(subframe, text='Description').grid(row=0, column=0)
    description_box.grid(row=1, column=0, columnspan=2, rowspan=3)


    # quantity
    Label(subframe, text='Quantity').grid(row=0, column=2)
    q = IntVar()
    quantity_box = Entry(subframe, text=q, width=4)
    quantity_box.grid(row=1, column=2)
    q.set(quant)
    quant_var = IntVar(root)
    
    def change_quantity(*args): # Is this necessary?
        quantity = quant_var.get()
        print(quantity)
    quant_var.trace('w', change_quantity)

    # price
    price_box = Entry(subframe, width=8)
    price_box.insert(0, str('%.2f' %price))
    Label(subframe, text='Price').grid(row=0, column=3)
    price_box.grid(row=1, column=3)


    # VAT or other - dropdown
    vat_options = ('No VAT', 'VAT: 20%', '5%')
    vatVar = StringVar(root)
    Label(subframe, text='Select tax').grid(row=0, column=4)
    tax_dropdown = OptionMenu(subframe, vatVar, *vat_options)
    tax_dropdown.grid(row=1, column=4)
    tax_mult = 0.2
    def change_tax_dropdown(*args):
        if vatVar.get() == vat_options[0]:
            tax_mult = 0.0
        elif vatVar.get() == vat_options[1]:
            tax_mult = 0.2
        elif vatVar.get() == vat_options[2]:
            tax_mult = 0.05
        print(vatVar.get())
    vatVar.trace('w', change_tax_dropdown)
    
    add_button = Button(subframe, text='Add', command=add_item).grid(row=1, column=5)


def generateInvoice():
    print (f'desc_items count = {len(desc_items)}')
    print (f'descriptions count = {len(descs)}')
    subtotal = 0.00
    vat = 0.00
    otherTax = 0.00
    for i in range(0, len(amounts)):
        if is_active[i] == True:
            subtotal += amounts[i]
            if tax_rate_strs[i] == '20%':
                vat += (0.2*amounts[i])
            elif tax_rate_strs[i] == '5%':
                otherTax += (0.05*amounts[i])
            
    total = subtotal+vat+otherTax


    customer = custVar.get()
    print(customer)
    #create invoicee address string
    cust_ind = customer_df[customer_df['customer_name'] == customer].index[0]
    cust_addr = ""
    for j in (13,14,15,16,12,11,6,1):
        if j == 6:
            cust_addr += "\n"
        # if pd.isnull(customer_df.iloc[cust_ind, j]) == False:
        if customer_df.iloc[cust_ind, j] != '':
            if j == 6:
                phone = str(customer_df.iloc[cust_ind, 6])
                phone = phone.replace('.0', '')
                cust_addr += (phone + '\n')
            else:
                cust_addr += (str(customer_df.iloc[cust_ind, j]) + "\n")
    # description = description_box.get("1.0",END)
    # print(description)

    
    inv_dt = datetime.date(year=1970, month=1, day=1)
    inv_dt = inv_dt_show.cget("text")
    inv_dt = datetime.datetime.strptime(inv_dt, '%Y-%m-%d')
    inv_dt = inv_dt.strftime('%d %B, %Y')

    due_dt = datetime.date(year=1970, month=1, day=1)
    due_dt = due_dt_show.cget("text")
    due_dt = datetime.datetime.strptime(due_dt, '%Y-%m-%d')
    due_dt = due_dt.strftime('%d %B, %Y')
    
    
    #MAKE PDF
    pdf.add_page()
    pdf.set_font('Arial', '', 28)
    pdf.cell(w=0, h=10, txt='INVOICE', border=0, ln=1, align='R')
    pdf.ln(4)


    #Invoicer name
    pdf.set_font('Arial', 'B', 10)
    pdf.cell(w=0, h=4, txt='Your Company', border=0, ln=1, align='R')
    pdf.set_font('Arial', '', 10)
    pdf.multi_cell(w=0, h=4, txt='1 Whatever Street\nAnytown\nAT1 3NZ\nUnited Kingdom\n\n07555 111111\nyourcompany@whatevermail.co.uk', border=0, ln=2, align='R')
    pdf.ln(4)
    


    #Left right split for invoicee details(left) and invoice no/amounts(right)
    ynow = pdf.get_y()
    effective_page_width = pdf.w - 2*pdf.l_margin
    pdf.line(pdf.l_margin, ynow, effective_page_width+pdf.l_margin, ynow)
    pdf.ln(4)
    
    ybefore = pdf.get_y()


    #left
    pdf.set_font('Arial', '', 10)
    pdf.cell(w=effective_page_width/2, h=4, txt='BILL TO', border=0, ln=1, align='L')
    pdf.set_font('Arial', 'B', 10)
    cust_str = customer_df.iloc[cust_ind, 0]
    cust_str = cust_str.replace("'", "\'") #unicode not recognising apostrophes needs sorting
    pdf.cell(w=effective_page_width/2, h=4, txt=cust_str , border=0, ln=1, align='L')
    pdf.set_font('Arial', '', 10)
    pdf.multi_cell(w=effective_page_width/2, h=4, txt=cust_addr, border=0, ln=1, align='L')
    yLeft = pdf.get_y()


    #inside right
    order = order_number_box.get()
    inv_num = inv_number_box.get()
    if order == '':
        lhText = 'Invoice Number: \nInvoice Date: \nPayment Due: \nAmount Due (GBP): \n'
        rhText = f'{inv_num}\n{inv_dt}\n{due_dt}\n£{"%.2f" %total}\n'
    else:
        lhText = 'Invoice Number: \nPO/SO Number: \nInvoice Date: \nPayment Due: \nAmount Due (GBP): \n'
        rhText = f'{inv_num}\n{order}\n{inv_dt}\n{due_dt}\n£{"%.2f" %total}\n'
    pdf.set_xy(3*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.set_font('Arial', 'B', 10)
    pdf.multi_cell(w=effective_page_width/5, h=8, txt=lhText, border=0, ln=1, align='R')


    #wide right
    pdf.set_xy(4*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.set_font('Arial', '', 10)
    pdf.multi_cell(w=effective_page_width/5, h=8, txt=rhText, border=0, ln=1, align='L')
    ybefore = pdf.get_y()
    pdf.set_font('Arial', 'B', 10)
    """pdf.set_xy(4*effective_page_width/5+pdf.l_margin, ybefore)
    # pdf.cell(w=effective_page_width/4, h=11, txt='£' + '%.2f' % gross_amount, border=0, ln=1, align='L')
    pdf.cell(w=effective_page_width/5, h=11, txt='£'+('%.2f' %total), border=0, ln=1, align='L')"""
    yRight = pdf.get_y()

    # line break
    print(f'yLeft={yLeft}')
    print(f'yRight={yRight}')
    ybefore = max(yLeft, yRight)
    pdf.set_xy(pdf.l_margin, ybefore)
    pdf.ln(4)
    ybefore = pdf.get_y()

    # Header 1 and 2 of 5
    pdf.set_text_color(255, 255, 255)
    pdf.set_fill_color(47, 47, 47)
    pdf.cell(w=2*effective_page_width/5, h=8, txt='Items', border=0, ln=1, align='L', fill=1)


    # Header 3 of 5
    pdf.set_xy(2*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=8, txt='Quantity', border=0, ln=1, align='C', fill=1)


    # Header 4 of 5
    pdf.set_xy(3*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=8, txt='Price', border=0, ln=1, align='R', fill=1)


    # Header 5 of 5
    pdf.set_xy(4*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=8, txt='Amount', border=0, ln=1, align='R', fill=1)

    
    # Body 1 and 2 of 5
    pdf.set_fill_color(255, 255, 255)
    pdf.set_text_color(0, 0, 0)
    pdf.set_font('Arial', '', 10)


    for i in range (0, len(desc_items)):
        if is_active[i] == True:
            print(descs[i])
            print(quantities[i])
            print(prices[i])

            # desrciption 1,2 / 5
            pdf.ln(4)
            ybefore = pdf.get_y()
            

            # quantity 3 of 5 quantity
            pdf.set_xy(2*effective_page_width/5+pdf.l_margin, ybefore)
            pdf.cell(w=effective_page_width/5, h=4, txt=str(quantities[i]), ln=1, align='C')


            # price - Body 4 of 5
            pdf.set_xy(3*effective_page_width/5+pdf.l_margin, ybefore)
            
            pdf.cell(w=effective_page_width/5, h=4, txt='£'+('%.2f' %prices[i]), ln=1, align='R')


            # amount - Body 5 of 5
            pdf.set_xy(4*effective_page_width/5+pdf.l_margin, ybefore)
            pdf.cell(w=effective_page_width/5, h=4, txt='£'+('%.2f' %amounts[i]), ln=1, align='R')


            # pdf.multi_cell(w=2*effective_page_width/5, h=4, txt=description, border=0, align='L',ln=1)
            pdf.set_xy(pdf.l_margin, ybefore)
            pdf.multi_cell(w=2*effective_page_width/5, h=4, txt=descs[i], border=0, ln=1, align='L')


            pdf.ln(4)
        


    pdf.ln(4)
    ynow = pdf.get_y()
    pdf.set_xy(pdf.l_margin, ynow)
    pdf.line(pdf.l_margin, ynow, effective_page_width+pdf.l_margin, ynow)
    pdf.ln(4)
    

    # subtotal
    ybefore = pdf.get_y()
    pdf.set_xy(3*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.set_font('Arial', 'B', 10)
    pdf.cell(w=effective_page_width/5, h=4, txt='Subtotal:', ln=1, align='R')
    # subtotal quantify
    pdf.set_xy(4*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.set_font('Arial', '', 10)
    pdf.cell(w=effective_page_width/5, h=4, txt='£'+('%.2f' %subtotal), ln=1, align='R')
    pdf.ln(4)


    # VAT
    ybefore = pdf.get_y()
    pdf.set_xy(3*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=4, txt='VAT 20%:', ln=1, align='R')
    #VAT quantify
    pdf.set_xy(4*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=4, txt='£'+('%.2f' %vat), ln=1, align='R')
    pdf.ln(4)


    # Other tax
    ybefore = pdf.get_y()
    pdf.set_xy(3*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=4, txt='Other tax:', ln=1, align='R')
    #Other tax quantify
    pdf.set_xy(4*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=4, txt='£'+('%.2f' %otherTax), ln=1, align='R')


    pdf.ln(4)
    ynow = pdf.get_y()
    pdf.line(3*effective_page_width/5+pdf.l_margin, ynow, effective_page_width+pdf.l_margin, ynow)
    pdf.ln(4)


    # Total
    ybefore = pdf.get_y()
    pdf.set_font('Arial', 'B', 10)
    pdf.set_xy(3*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=4, txt='Total:', ln=1, align='R')
    # Total quantify
    pdf.set_xy(4*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.set_font('Arial', '', 10)
    pdf.cell(w=effective_page_width/5, h=4, txt='£'+('%.2f' %total), ln=1, align='R')


    pdf.ln(4)
    ynow = pdf.get_y()
    pdf.line(3*effective_page_width/5+pdf.l_margin, ynow, effective_page_width+pdf.l_margin, ynow)
    pdf.ln(4)


    # Amount due
    ybefore = pdf.get_y()
    pdf.set_font('Arial', 'B', 10)
    pdf.set_xy(3*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=4, txt='Amount Due (GBP):', ln=1, align='R')
    # Amount due quantify
    pdf.set_xy(4*effective_page_width/5+pdf.l_margin, ybefore)
    pdf.cell(w=effective_page_width/5, h=4, txt='£'+('%.2f' %total), ln=1, align='R')


    # Notes / Terms
    ybefore = pdf.get_y()
    pdf.set_xy(pdf.l_margin, ybefore)
    pdf.ln(8)
    pdf.cell(w=effective_page_width, h=4, txt='Notes / Terms', ln=1, align='L')
    pdf.set_font('Arial', '', 10)
    notesTermsStr = 'Your Company account details\nAccount name: Your Name T/A Your Company\nAccount no: 12345678\nSort code: 65-43-21'
    pdf.multi_cell(w=effective_page_width, h=4, txt=notesTermsStr, border=0, ln=1, align='L')


    pdf.set_y(-25)
    pdf.cell(w=effective_page_width, h=4, txt='VAT: 123 4567 89', border=0, ln=1, align='C')


    #write pdf
    pdf.output('fpdfdemo.pdf', 'F')

    invGuiFrame.quit()



# Create main window
root = Tk()
invGuiFrame = Frame(root)
invGuiFrame.grid(column=0, row=0, sticky=(N,W,E,S))
invGuiFrame.columnconfigure(0, weight=1)
invGuiFrame.rowconfigure(0, weight=1)
invGuiFrame.pack(pady=10, padx=10)


# fetch customers from file and put into collection
custVar = StringVar(root)
customer_df = pd.read_csv('Data/customers2.csv')
customer_df = customer_df.fillna('') #eliminate 'na'
# tkvar.set(customer_df.iloc[0,0])  # if we want to set default
custMenu = OptionMenu(invGuiFrame, custVar, *customer_df['customer_name'])
Label(invGuiFrame, text='Choose customer').grid(row=0, column=0)
custMenu.grid(row = 1, column = 0)


# invoice date button
inv_dt_btn = Button(invGuiFrame, text='Invoice date', command=partial(setDate, 'Set invoice date'))
inv_dt_btn.grid(row=0, column=1)
# invoice date label
inv_dt_show = Label(invGuiFrame, text='Not selected') # get variable from here
# inv_dt_show = Text(invGuiFrame, width=20, height=1)
inv_dt_show.grid(row=1, column=1)


# due date button
due_dt_btn = Button(invGuiFrame, text='Due date', command=partial(setDate, 'Set due date'))
due_dt_btn.grid(row=0, column=2)
# due date label
due_dt_show = Label(invGuiFrame, text='Not selected') # get variable from here
# due_dt_show = Text(invGuiFrame, width=20, height=1)
due_dt_show.grid(row=1, column=2)


# PO number
Label(invGuiFrame, text='Order number').grid(row=0, column=3)
order_number_box = Entry(invGuiFrame, text='Order number')
order_number_box.grid(row=1, column=3)


# Invoice number
Label(invGuiFrame, text='Invoice number').grid(row=0, column=5)
inv_number_box = Entry(invGuiFrame)
inv_number_box.grid(row=1, column=5)
inv_number_box.insert(0, '107790-')


desc_items = []
descs = []
quantity_items = []
quantities = []
price_items = []
prices = []
amount_items = []
amounts = []
tax_percents = []
tax_mults = []
tax_rate_strs = []
edit_btns = []
delete_btns = []
is_active = []


# submit_btn=Button(invGuiFrame, text='Submit', command=generateInvoice)
item_ct = 0
# add_item_btn = Button(invGuiFrame, text='Add item', command=partial(item_creator, '', 1, 0))
add_item_btn = Button(invGuiFrame, text='Add item', command=partial(item_creator, True, 0))
add_item_btn.grid(row=2, column=5)


submit_btn = Button(invGuiFrame, text='Generate PDF', command=generateInvoice).grid(row=3, column=5)


# execute
root.mainloop()