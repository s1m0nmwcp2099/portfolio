import pandas as pd
import numpy as np
import mysql.connector
from datetime import timedelta


class Form:
    def __init__(self, ftg, ftr, htg, htr, sht_cor):
        self.ftg = ftg    # full time goals - tuple: (average goals for, average goals against)
        self.ftr = ftr    # full time results - tuple: (win proportin, draw proportion, loss proportion)
        self.htg = htg    # half time goals - tuple
        self.htr = htr    # half time results - tuple
        self.sht_cor = sht_cor # shots and corners - tuple: (shots on target for, sot against, shots for, s against, corners for, c against)


class Team:
    def __init__(self, name, home_form, away_form):
        self.name = name
        self.home_form = home_form
        self.away_form = away_form


def team_averages_per_game(team, opp_team, this_match_date, earliest_date, at_home, df, min_games):
    if at_home == True:
        adj = 0 # adjuster
        h_or_a = 'HomeTeam'
    else:
        adj = 1
        h_or_a = 'AwayTeam'
        
    
    df_team = df.loc[(df['Date']>earliest_date) & (df['Date']<this_match_date) & (df[h_or_a]==team)]
    
    if len(df_team) >= min_games:
        # team_averages = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0]
        team_averages = {
            'ftg_for': 0.0,  # full time goals for
            'ftg_ag': 0.0,   # full time goals against
            'ftw': 0.0,      # full time win
            'ftd': 0.0,      # full time draw
            'ftl': 0.0,      # full time loss
            'htg_for': 0.0,  # half time goals for
            'htg_ag': 0.0,   # half time goals against
            'htw': 0.0,      # half time win
            'htd': 0.0,      # half time draw
            'htl': 0.0,      # half time loss
            'sot_for': 0.0,  # shots on target for
            'sot_ag': 0.0,   # shots on target against
            's_for': 0.0,    # shots for
            's_ag': 0.0,     # shots against
            'c_for': 0.0,    # corners for
            'c_ag': 0.0      # corners against
        }
        pld_weight = 0.0 # number of games played represented as time-weighted units


        for i in range (0, len(df_team)):
            that_match_date = df_team.iloc[i]['Date'] # date of previous match
            days_gap = float((this_match_date - that_match_date).days)
            time_coeff = np.exp(-0.007 * days_gap)
            relevance = time_coeff
            if df_team.iloc[i, 3-adj] == opp_team: #is this previous match's opposition the same as today's?
                relevance *= 1.5
            pld_weight += relevance

            team_averages['ftg_for'] += (df_team.iloc[i, 4+adj] * relevance)
            team_averages['ftg_ag'] += (df_team.iloc[i, 5-adj] * relevance)
            if (df_team.iloc[i, 6]=='H' and at_home==True) or (df_team.iloc[i, 6]=='A' and at_home==False):
                team_averages['ftw'] += relevance                    
            elif df_team.iloc[i, 6] == 'D':
                team_averages['ftd'] += relevance                    
            elif (df_team.iloc[i, 6]=='A' and at_home==True) or (df_team.iloc[i, 6]=='H' and at_home==False): # perhaps just 'else' but this feels right
                team_averages['ftl'] += relevance                    
            team_averages['htg_for'] += (df_team.iloc[i, 7+adj] * relevance)  
            team_averages['htg_ag'] += (df_team.iloc[i, 8-adj] * relevance)  
            if (df_team.iloc[i, 9]=='H' and at_home==True) or (df_team.iloc[i, 9]=='A' and at_home==False):
                team_averages['htw'] += relevance                    
            elif df_team.iloc[i, 9] == 'D':
                team_averages['htd'] += relevance                    
            elif (df_team.iloc[i, 9]=='A' and at_home==True) or (df_team.iloc[i, 9]=='H' and at_home==False):
                team_averages['htl'] += relevance                    
            team_averages['sot_for'] += (df_team.iloc[i, 10+adj] * relevance)
            team_averages['sot_ag'] += (df_team.iloc[i, 11-adj] * relevance)
            team_averages['s_for'] += (df_team.iloc[i, 12+adj] * relevance)
            team_averages['s_ag'] += (df_team.iloc[i, 13-adj] * relevance)
            team_averages['c_for'] += (df_team.iloc[i, 14+adj] * relevance)
            team_averages['c_ag'] += (df_team.iloc[i, 15-adj] * relevance)

        # divide
        tm_av_key_list = list(team_averages.keys())
        for j in range(0, len(tm_av_key_list)):
            team_averages[tm_av_key_list[j]] /= pld_weight
        return team_averages
    else:
        return None


## ENTRY POINT

min_games = 6


conn_str = mysql.connector.connect(
    host = 'localhost',
    user = 'simon',
    password = 'chainsaw',
    database = 'football'
)


query = "SELECT ThisDiv, Date, HomeTeam, AwayTeam, FTHG, FTAG, FTR, HTHG, HTAG, HTR, HST, AwST, HS, AwS, HC, AC FROM football_data_complete ORDER BY Date;"
print('Fetching from sql')
df_raw = pd.read_sql_query(query, conn_str)
print('Fetched from mysql')
conn_str.close()


df_raw = df_raw.dropna()
for i in range (0, len(df_raw)):
    match_date = df_raw.iloc[i]['Date']
    earliest_date = match_date - timedelta(days=180)
    # print(str(df_raw.iloc[i]['Date']) + ' ' + str(df_raw.iloc[i]['HomeTeam']) + ' v ' + str(df_raw.iloc[i]['AwayTeam']) + ' ' + str(df_raw.iloc[0]['FTR']))
    if earliest_date > df_raw.iloc[0]['Date']:
        home_team, away_team = df_raw.iloc[i]['HomeTeam'], df_raw.iloc[i]['AwayTeam']

        home_team_home_form = team_averages_per_game(home_team, away_team, match_date, earliest_date, True, df_raw, min_games)
        home_team_away_form = team_averages_per_game(home_team, away_team, match_date, earliest_date, False, df_raw, min_games)

        away_team_away_form = team_averages_per_game(away_team, home_team, match_date, earliest_date, False, df_raw, min_games)
        away_team_home_form = team_averages_per_game(away_team, home_team, match_date, earliest_date, True, df_raw, min_games)

        print(home_team_home_form)
        print(home_team_away_form)
        print(away_team_away_form)
        print(away_team_home_form)