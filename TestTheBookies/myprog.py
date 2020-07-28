import pandas as pd
import numpy as np
import mysql.connector

mydb = mysql.connector.connect(host = "localhost", user = "simon", password = "chainsaw", database = "football")
mycursor = mydb.cursor()
mycursor.execute("SELECT * FROM results_and_odds;")
results = pd.DataFrame(mycursor.fetchall())

success = 0
for i in range(0, len(results)):
    hm_gls = int(results.iloc[i, 4])
    aw_gls = int(results.iloc[i, 5])
    hm_odds = float(results.iloc[i, 6])
    dr_odds = float(results.iloc[i, 7])
    aw_odds = float(results.iloc[i, 8])
    min_odds = min(hm_odds, dr_odds, aw_odds)

    if hm_odds == min_odds and hm_gls > aw_gls:
        if hm_odds == dr_odds or hm_odds == aw_odds:
            success += 1
        else:
            success += 2
    elif dr_odds == min_odds and hm_gls == aw_gls:
        if dr_odds == hm_odds or dr_odds == aw_odds:
            success += 1
        else:
            success += 2
    elif aw_odds == min_odds and hm_gls < aw_gls:
        if aw_odds == hm_odds or aw_odds ==dr_odds:
            success += 1
        else:
            success += 2
success /= 2
accuracy = success / len(results)
print(accuracy)