import pandas as pd
import numpy as np
import mysql.connector
from datetime import timedelta


class Form:
    def __init__(self, ftg, ftr, htg, htr, sht_cor, streak):
        self.ftg = ftg    # full time goals - tuple: (average goals for, average goals against)
        self.ftr = ftr    # full time results - tuple: (win proportin, draw proportion, loss proportion)
        self.htg = htg    # half time goals - tuple
        self.htr = htr    # half time results - tuple
        self.sht_cor = sht_cor # shots and corners - tuple: (shots on target for, sot against, shots for, s against, corners for, c against)
        self.streak = streak # tuple: (w, wx, d, dx, l, lx)


class Team:
    def __init__(self, name, home_form, away_form):
        self.name = name
        self.home_form = home_form
        self.away_form = away_form


def team_averages_per_game_and_runs(team, opp_team, this_match_date, earliest_date, at_home, df, min_games, this_div):
    if at_home == True:
        adj = 0 # adjuster
        h_or_a = 'HomeTeam'
    else:
        adj = 1
        h_or_a = 'AwayTeam'
        
    
    df_team = df.loc[(df['Date']>earliest_date) & (df['Date']<this_match_date) & (df[h_or_a]==team)]
    
    if len(df_team) >= min_games:
        team_averages_and_runs = {
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
            'c_ag': 0.0,     # corners against
            'winning_run': 0,    # runs
            'winless_run': 0, 
            'drawing_run': 0,
            'drawless_run': 0,
            'losing_run': 0,
            'lossless_run': 0
        }
        pld_weight = 0.0 # number of games played represented as time-weighted units

        w_run, wx_run, d_run, dx_run, l_run, lx_run = True, True, True, True, True, True

        for i in range (len(df_team)-1, -1, -1):
            that_match_date = df_team.iloc[i]['Date'] # date of previous match
            days_gap = float((this_match_date - that_match_date).days)
            time_coeff = np.exp(-0.007 * days_gap)
            relevance = time_coeff
            if df_team.iloc[i, 3-adj] == opp_team: #is this previous match's opposition the same as today's?
                relevance *= 1.5
            if this_div != df_team.iloc[i]['ThisDiv']: # different division?
                relevance *= 0.5
            pld_weight += relevance

            team_averages_and_runs['ftg_for'] += (df_team.iloc[i, 4+adj] * relevance)
            team_averages_and_runs['ftg_ag'] += (df_team.iloc[i, 5-adj] * relevance)
            if (df_team.iloc[i, 6]=='H' and at_home==True) or (df_team.iloc[i, 6]=='A' and at_home==False):
                team_averages_and_runs['ftw'] += relevance
                if w_run == True:
                    team_averages_and_runs['winning_run'] += 1
                if dx_run == True:
                    team_averages_and_runs['drawless_run'] += 1
                if lx_run == True:
                    team_averages_and_runs['lossless_run'] += 1
                wx_run = False
                d_run = False
                l_run = False
            elif df_team.iloc[i, 6] == 'D':
                team_averages_and_runs['ftd'] += relevance
                if d_run == True:
                    team_averages_and_runs['drawing_run'] += 1
                if wx_run == True:
                    team_averages_and_runs['winless_run'] += 1
                if lx_run == True:
                    team_averages_and_runs['lossless_run'] += 1
                w_run = False
                dx_run = False
                l_run = False                  
            elif (df_team.iloc[i, 6]=='A' and at_home==True) or (df_team.iloc[i, 6]=='H' and at_home==False): # perhaps just 'else' but this feels right
                team_averages_and_runs['ftl'] += relevance
                if l_run == True:
                    team_averages_and_runs['losing_run'] += 1
                if wx_run == True:
                    team_averages_and_runs['winless_run'] += 1
                if dx_run == True:
                    team_averages_and_runs['drawless_run'] += 1
                w_run = False
                d_run = False
                lx_run = False                   
            team_averages_and_runs['htg_for'] += (df_team.iloc[i, 7+adj] * relevance)  
            team_averages_and_runs['htg_ag'] += (df_team.iloc[i, 8-adj] * relevance)  
            if (df_team.iloc[i, 9]=='H' and at_home==True) or (df_team.iloc[i, 9]=='A' and at_home==False):
                team_averages_and_runs['htw'] += relevance                    
            elif df_team.iloc[i, 9] == 'D':
                team_averages_and_runs['htd'] += relevance                    
            elif (df_team.iloc[i, 9]=='A' and at_home==True) or (df_team.iloc[i, 9]=='H' and at_home==False):
                team_averages_and_runs['htl'] += relevance                    
            team_averages_and_runs['sot_for'] += (df_team.iloc[i, 10+adj] * relevance)
            team_averages_and_runs['sot_ag'] += (df_team.iloc[i, 11-adj] * relevance)
            team_averages_and_runs['s_for'] += (df_team.iloc[i, 12+adj] * relevance)
            team_averages_and_runs['s_ag'] += (df_team.iloc[i, 13-adj] * relevance)
            team_averages_and_runs['c_for'] += (df_team.iloc[i, 14+adj] * relevance)
            team_averages_and_runs['c_ag'] += (df_team.iloc[i, 15-adj] * relevance)

        # divide
        tm_av_key_list = list(team_averages_and_runs.keys())
        for j in range(0, len(tm_av_key_list)-6): # -6 to leave streaks
            team_averages_and_runs[tm_av_key_list[j]] /= pld_weight
        return team_averages_and_runs
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

lg = []
match_dt = []
home = []
away = []
ft_result = []

for i in range (0, len(df_raw)):
    match_date = df_raw.iloc[i]['Date']
    this_div = df_raw.iloc[i]['ThisDiv']
    earliest_date = match_date - timedelta(days=180)
    # print(str(df_raw.iloc[i]['Date']) + ' ' + str(df_raw.iloc[i]['HomeTeam']) + ' v ' + str(df_raw.iloc[i]['AwayTeam']) + ' ' + str(df_raw.iloc[0]['FTR']))
    if earliest_date > df_raw.iloc[0]['Date']:
        home_team, away_team = df_raw.iloc[i]['HomeTeam'], df_raw.iloc[i]['AwayTeam']

        hthf = team_averages_per_game_and_runs(home_team, away_team, match_date, earliest_date, True, df_raw, min_games, this_div)
        htaf = team_averages_per_game_and_runs(home_team, away_team, match_date, earliest_date, False, df_raw, min_games, this_div)
        ataf = team_averages_per_game_and_runs(away_team, home_team, match_date, earliest_date, False, df_raw, min_games, this_div)
        athf = team_averages_per_game_and_runs(away_team, home_team, match_date, earliest_date, True, df_raw, min_games, this_div)
        
        print(f'{match_date} - {home_team} v {away_team}')
        if hthf != None and htaf != None and ataf != None and athf != None:
            h_home_form = Form((hthf['ftg_for'], hthf['ftg_ag']), (hthf['ftw'], hthf['ftd'], hthf['ftl']), (hthf['htg_for'], hthf['htg_ag']), (hthf['htw'], hthf['htd'], hthf['htl']), (hthf['sot_for'], hthf['sot_ag'], hthf['s_for'], hthf['s_ag'], hthf['c_for'], hthf['c_ag']), (hthf['winning_run'], hthf['winless_run'], hthf['drawing_run'], hthf['drawless_run'], hthf['losing_run'], hthf['lossless_run']))
            h_away_form = Form((htaf['ftg_for'], htaf['ftg_ag']), (htaf['ftw'], htaf['ftd'], htaf['ftl']), (htaf['htg_for'], htaf['htg_ag']), (htaf['htw'], htaf['htd'], htaf['htl']), (htaf['sot_for'], htaf['sot_ag'], htaf['s_for'], htaf['s_ag'], htaf['c_for'], htaf['c_ag']), (htaf['winning_run'], htaf['winless_run'], htaf['drawing_run'], htaf['drawless_run'], htaf['losing_run'], htaf['lossless_run']))
            a_away_form = Form((ataf['ftg_for'], ataf['ftg_ag']), (ataf['ftw'], ataf['ftd'], ataf['ftl']), (ataf['htg_for'], ataf['htg_ag']), (ataf['htw'], ataf['htd'], ataf['htl']), (ataf['sot_for'], ataf['sot_ag'], ataf['s_for'], ataf['s_ag'], ataf['c_for'], ataf['c_ag']), (ataf['winning_run'], ataf['winless_run'], ataf['drawing_run'], ataf['drawless_run'], ataf['losing_run'], ataf['lossless_run']))
            a_home_form = Form((athf['ftg_for'], athf['ftg_ag']), (athf['ftw'], athf['ftd'], athf['ftl']), (athf['htg_for'], athf['htg_ag']), (athf['htw'], athf['htd'], athf['htl']), (athf['sot_for'], athf['sot_ag'], athf['s_for'], athf['s_ag'], athf['c_for'], athf['c_ag']), (athf['winning_run'], athf['winless_run'], athf['drawing_run'], athf['drawless_run'], athf['losing_run'], athf['lossless_run']))

            t1 = Team(home_team, h_home_form, h_away_form)
            t2 = Team(away_team, a_home_form, a_away_form)

            lg.append(this_div)
            match_dt.append(match_date)
            home.append(t1)
            away.append(t2)

data_1st = {'ThisDiv': lg, 'Date': match_dt, 'HomeTeam': home, 'AwayTeam': away}
df_1st_process = pd.DataFrame(data_1st)        