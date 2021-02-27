import pandas as pd
import numpy as np
import mysql.connector
from datetime import timedelta


"""
team_averages indexes
0 = average full time goals scored per game
1 = average full time goals conceded per game
2 = average full time wins per game
3 = average full time draws per game
4 = average full time losses per game
5 = average half time goals scored per game
6 = average half time goals conceded per game
7 = average half time wins per game
8 = average half time draws per game
9 = average half time losses per game
10 = average shots on target for per game
11 = average shots on target against per game
12 = average shots for per game
13 = average shots against per game
14 = average corners for per game
15 = average corners against per game
"""

def team_averages_per_game(df, at_home, this_match_date): # going back 365 days, i = row index - These are time weighted
    team_averages = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0]
    pld_weight = 0.0 # number of games played represented as time-weighted units
    if at_home == True:
        adj = 0 # adjuster
        h_or_a = 'HomeTeam'
    else:
        adj = 1
        h_or_a = 'AwayTeam'
    for i in range (0, len(df)):
        that_match_date = df.iloc[i, 1] # date of previous match
        days_gap = float((this_match_date - that_match_date).days)
        time_coeff = np.exp(-0.007 * days_gap)
        pld_weight += time_coeff

        team_averages[0] += (df.iloc[i, 4+adj] * time_coeff)  # full time goals for
        team_averages[1] += (df.iloc[i, 5-adj] * time_coeff)  # full time goals against
        if (df.iloc[i, 6]=='H' and at_home==True) or (df.iloc[i, 6]=='A' and at_home==False):
            team_averages[2] += time_coeff                    # full time win
        elif df.iloc[i, 6] == 'D':
            team_averages[3] += time_coeff                    # full time draw
        elif (df.iloc[i, 6]=='A' and at_home==True) or (df.iloc[i, 6]=='H' and at_home==False):
            team_averages[4] += time_coeff                    # full time loss
        team_averages[5] += (df.iloc[i, 7+adj] * time_coeff)  # half time goals for
        team_averages[6] += (df.iloc[i, 8-adj] * time_coeff)  # half time goals against
        if (df.iloc[i, 9]=='H' and at_home==True) or (df.iloc[i, 9]=='A' and at_home==False):
            team_averages[7] += time_coeff                    # full time win
        elif df.iloc[i, 9] == 'D':
            team_averages[8] += time_coeff                    # full time draw
        elif (df.iloc[i, 9]=='A' and at_home==True) or (df.iloc[i, 9]=='H' and at_home==False):
            team_averages[9] += time_coeff                    # full time loss
        team_averages[10] += (df.iloc[i, 10+adj] * time_coeff)# shots on target for
        team_averages[11] += (df.iloc[i, 11-adj] * time_coeff)# shots on target against
        team_averages[12] += (df.iloc[i, 12+adj] * time_coeff)# shots for
        team_averages[13] += (df.iloc[i, 13-adj] * time_coeff)# shots against
        team_averages[14] += (df.iloc[i, 14+adj] * time_coeff)# corners for
        team_averages[15] += (df.iloc[i, 15-adj] * time_coeff)# corners against

    for i in range (0, len(team_averages)):
        team_averages[i] /= pld_weight

    return team_averages


## ENTRY POINT


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


# get averages
all_averages = [df_raw['FTHG'].mean(), df_raw['FTAG'].mean(), df_raw['HTHG'].mean(), df_raw['HTAG'].mean(),
        df_raw['HST'].mean(), df_raw['AwST'].mean(), df_raw['HS'].mean(), df_raw['AwS'].mean(), df_raw['HC'].mean(), df_raw['AC'].mean()]



df_raw = df_raw.dropna()
earliest = df_raw.iloc[0]['Date']
for i in range (0, len(df_raw.index)):
    this_date = df_raw.iloc[i]['Date']
    one_year_earlier = this_date - timedelta(days=365)
    if this_date > (earliest + timedelta(days=365)):
        # calculate home averages
        home_team = df_raw.iloc[i]['HomeTeam']
        df_home_team_prev_at_home = df_raw.loc[(df_raw['HomeTeam']==home_team) & (df_raw['Date']<this_date) & (df_raw['Date']>one_year_earlier)]
        home_team_home_averages = team_averages_per_game(df_home_team_prev_at_home, True, this_date)
        df_home_team_prev_at_away = df_raw.loc[(df_raw['AwayTeam']==home_team) & (df_raw['Date']<this_date) & (df_raw['Date']>one_year_earlier)]
        home_team_away_averages = team_averages_per_game(df_home_team_prev_at_away, False, this_date)

        # calculate away averages
        away_team = df_raw.iloc[i]['AwayTeam']
        df_away_team_prev_at_away = df_raw.loc[(df_raw['AwayTeam']==away_team) & (df_raw['Date']<this_date) & (df_raw['Date']>one_year_earlier)]
        away_team_away_averages = team_averages_per_game(df_away_team_prev_at_away, False, this_date)
        df_away_team_prev_at_home = df_raw.loc[(df_raw['HomeTeam']==away_team) & (df_raw['Date']<this_date) & (df_raw['Date']>one_year_earlier)]
        away_team_home_averages = team_averages_per_game(df_away_team_prev_at_home, True, this_date)
        
        # calculate league averages
        df_lg_year = df_raw.loc[(df_raw['Date']<this_date) & (df_raw['Date']>one_year_earlier)]
        home_all_av = team_averages_per_game(df_lg_year, True, this_date)
        away_all_av = team_averages_per_game(df_lg_year, False, this_date)