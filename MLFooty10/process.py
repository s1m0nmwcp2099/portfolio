import pandas as pd
import numpy as np
import mysql.connector
from datetime import timedelta


conn_str = mysql.connector.connect(
    host = 'localhost',
    user = 'simon',
    password = 'chainsaw',
    database = 'football'
)


query = "SELECT ThisDiv, Date, HomeTeam, AwayTeam, FTHG, FTAG, HTHG, HTAG, HST, AwST, HS, AwS, HC, AC FROM football_data_complete ORDER BY Date;"
print('Fetching from sql')
df_raw = pd.read_sql_query(query, conn_str)
print('Fetched from mysql')
conn_str.close()


print(df_raw.head())


df_raw = df_raw.dropna()
earliest = df_raw.iloc[0]['Date']
for i in range (0, len(df_raw.index)):
    if df_raw.iloc[i]['Date'] > (earliest + timedelta(days=365)):
        print(df_raw.iloc[i])
        input()