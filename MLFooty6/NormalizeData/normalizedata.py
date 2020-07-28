import pandas as pd
import numpy as np

#fetch data
df = pd.read_csv("../Data/historical_match_data_with_headers.csv")

#get max average goals
av_gls = df[["hm_gls_for", "hm_gls_con", "aw_gls_for", "aw_gls_con"]].to_numpy()
max_av_gls = np.amax(av_gls)
print(max_av_gls)

#get max average opposition goals
av_opp_gls = df[["hm_opp_gls_for", "hm_opp_gls_con", "aw_opp_gls_for", "aw_opp_gls_con"]].to_numpy()
max_av_opp_gls = np.amax(av_opp_gls)
print(max_av_opp_gls)

#save maximums to file for normalizing fixture info
maxs = [str(max_av_gls), str(max_av_opp_gls)]
max_csv_line = ','.join(maxs)
print(max_csv_line)
df_maxs = pd.DataFrame(maxs)
df_maxs.to_csv('../Data/maximums_for_normalization.csv')

#print (df)

#normalize historic result data
df[["hm_gls_for", "hm_gls_con", "aw_gls_for", "aw_gls_con"]] /= max_av_gls
df[["hm_opp_gls_for", "hm_opp_gls_con", "aw_opp_gls_for", "aw_opp_gls_con"]] /= max_av_opp_gls
df[["hm_odds", "dr_odds", "aw_odds"]] = 1 / df[["hm_odds", "dr_odds", "aw_odds"]]
df = df.drop("Unnamed: 0", axis = 1)

#print (df)

#write to file
df.to_csv('../Data/normalized_historical_match_data_with_headers.csv', index = False)