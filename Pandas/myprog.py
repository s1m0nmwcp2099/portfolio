import pandas as pd
import mysql.connector
import numpy as np

my_list = []
my_list.append("a,b,c")
my_list.append("d,e,f")

df = pd.DataFrame(sub.split(',') for sub in my_list)
print (df)
df.to_csv('../x_data/this.csv')