import requests
from bs4 import BeautifulSoup
import json
import numpy as np
import pandas as pd
import csv
import mysql.connector
from datetime import date, datetime as dt


#Get fixtures
url1 = 'https://www.football-data.co.uk/fixtures.csv'
url2 = 'https://www.football-data.co.uk/new_league_fixtures.csv'
r1 = requests.get(url1)
r2 = requests.get(url2)

f_name_fx1 = 'Data/fixtures1.csv'
with open(f_name_fx1, 'wb') as f:
    f.write(r1.content)

f_name_fx2 = 'Data/fixtures2.csv'
with open(f_name_fx2, 'wb') as f:
    f.write(r2.content)

df_fx1 = pd.read_csv(f_name_fx1)

df_fx2 = pd.read_csv(f_name_fx2)
df_fx2 = df_fx2[df_fx2['Country'] == 'Russia']

leagues = df_fx1['Div'].to_list() + df_fx2['Country'].to_list()
these_dates = df_fx1['Date'].to_list() + df_fx2['Date'].to_list()
home_teams = df_fx1['HomeTeam'].to_list() + df_fx2['Home'].to_list()
away_teams = df_fx1['AwayTeam'].to_list() + df_fx2['Away'].to_list()
home_odds = df_fx1['B365H'].to_list() + df_fx2['PH'].to_list()
home_odds_max = df_fx1['MaxH'].to_list() + df_fx2['MaxH'].to_list()
draw_odds = df_fx1['B365D'].to_list() + df_fx2['PD'].to_list()
draw_odds_max = df_fx1['MaxD'].to_list() + df_fx2['MaxD'].to_list()
away_odds = df_fx1['B365A'].to_list() + df_fx2['PA'].to_list()
away_odds_max = df_fx1['MaxA'].to_list() + df_fx2['MaxA'].to_list()

df_fx = pd.DataFrame()
df_fx['league'] = leagues
df_fx['date'] = these_dates
df_fx['date'] = pd.to_datetime(df_fx['date'], dayfirst = True)
df_fx['home_team'] = home_teams
df_fx['away_team'] = away_teams
df_fx['home_odds'] = home_odds
df_fx['home_odds_max'] = home_odds_max
df_fx['draw_odds'] = draw_odds
df_fx['draw_odds_max'] = draw_odds_max
df_fx['away_odds'] = away_odds
df_fx['away_odds_max'] = away_odds_max

df_fx.to_csv('Data/fixtures_all.csv')
print('Fixtures combined')
earliest_fx_date = min(df_fx['date'])


base_url = 'https://understat.com/league' 
leagues = ['La_liga', 'EPL', 'Bundesliga', 'Serie_A', 'Ligue_1', 'RFPL'] 
seasons = ['2014', '2015', '2016', '2017', '2018', '2019' '2020', '2021']


#SCRAPE THIS AND LAST SEASON
df_prev = pd.DataFrame()


for lg in leagues:
    for sn in seasons:
        url = base_url+'/'+lg+'/'+sn
        print(url)


        res = requests.get(url)
        # page = requests.get(url)
        soup = BeautifulSoup(res.content, 'xml')
        scripts = soup.find_all('script')
        string_with_json_obj = ''
        ct = 0
        for el in scripts:
            if 'teamsData' in el.text:
                string_with_json_obj = el.text.strip()
                # print(string_with_json_obj)
                ct += 1
        print(f'{ct} matches scraped')


        ind_start = string_with_json_obj.index("('")+2
        ind_end = string_with_json_obj.index("')")
        json_data = string_with_json_obj[ind_start:ind_end]
        json_data = json_data.encode('utf8').decode('unicode_escape')
        # print(json_data)
        print(type(json_data))


        data = json.loads(json_data)
        print(type(data))


        teams = {}
        for id in data.keys():
            teams[id] = data[id]['title']


        columns = []
        values = []
        for id in data.keys():
            columns = list(data[id]['history'][0].keys())
            values = list(data[id]['history'][0].values())
            break


        dataframes = {}
        for id, team in teams.items():
            teams_data = []
            for row in data[id]['history']:
                teams_data.append(list(row.values()))
            df = pd.DataFrame(teams_data, columns=columns)
            dataframes[team] = df
            print('Added data for {}.'.format(team))


        # print(dataframes['Barcelona'].head(4))


        for team, df in dataframes.items():
            dataframes[team]['ppda_coef'] = dataframes[team]['ppda'].apply(lambda x: x['att']/x['def'] if x['def'] != 0 else 0)
            dataframes[team]['oppda_coef'] = dataframes[team]['ppda_allowed'].apply(lambda x: x['att']/x['def'] if x['def'] != 0 else 0)
            df['Team'] = team
            df['League'] = lg
            #f_name = 'Data/{}-{}-{}.csv'.format(lg, sn, team)
            #df.to_csv(f_name)
            if (df_prev.empty):
                df_prev = df
            else:
                frames = [df_prev, df]
                df_prev = pd.concat(frames)
f_name = 'Data/previous_match_data.csv'
#df_prev.to_csv(f_name)




alt_names = pd.read_csv('Data/team_map.csv', index_col=0, squeeze=True).to_dict()
def map_team(team):
    if team in alt_names:
        return alt_names[team]
    else:
        return team
df_prev = pd.read_csv('Data/previous_match_data.csv')
df_prev['date'] = pd.to_datetime(df_prev['date'])
td = pd.Timedelta('11 days')
df_prev.loc[(df_prev['date'] == pd.to_datetime('2018-04-14 18:00:00')) & (df_prev['Team'] == 'Caen'), 'date'] = pd.to_datetime('2018-04-25 18:00:00')
df_prev = df_prev.sort_values(by=['date'])
df_prev['Team'] = df_prev['Team'].apply(map_team)
df_prev.to_csv('Data/previous_match_data_sorted.csv')


# GET AWAY TEAMS
alt_names = pd.read_csv('Data/team_map.csv', index_col=0, squeeze=True).to_dict()
def map_team(team):
    if team in alt_names:
        return alt_names[team]
    else:
        return team


conn_str = mysql.connector.connect(
    host = 'localhost',
    user = 'simon',
    password = 'chainsaw',
    database = 'football'
)


def get_away_team(home_team, date):
    query = f"SELECT AwayTeam FROM football_data_complete WHERE (HomeTeam = '{home_team}' AND Date >= DATE_SUB('{date}', INTERVAL 1 DAY) AND Date <= DATE_ADD('{date}', INTERVAL 1 DAY));"
    #query = f"SELECT AwayTeam FROM football_data_complete WHERE (HomeTeam = '{home_team}' AND Date = '{date}');"
    cursor_obj = conn_str.cursor()
    cursor_obj.execute(query)
    aw_tm = str(cursor_obj.fetchall())
    aw_tm = aw_tm.strip("[('")
    aw_tm = aw_tm.strip("',)]")
    # print(type(aw_tm))
    if (len(aw_tm) > 0):
        # print(aw_tm + ' ' + date)
        return aw_tm
    else:
        print(f'ERROR NO AWAY TEAM RETURNED for {home_team} on {date}')
        return 'Null'


df_prev = df_prev[(df_prev['h_a'] == 'h') & (df_prev['date'] < earliest_fx_date)]


away_teams = []
ct = 0
print('Fetching away teams')
df_prev['date'] = df_prev['date'].dt.date
for index, row in df_prev.iterrows():
    home_team = row['Team']
    #this_date = str(row['date'])
    #this_date = dt.strptime(this_date, '%Y-%m-%d %H:%M:%S')
    #this_date = dt.strftime(this_date, '%Y-%m-%d')
    this_date = str(row['date'])
    away_team = get_away_team(home_team, this_date)
    away_teams.append(away_team)
    ct +=1
    if ct % 100 == 0:
        print(f'{ct} away teams fetched - {this_date}')


df_prev_match_completed = pd.DataFrame()
df_prev_match_completed['league'] = df_prev['League']
df_prev_match_completed['date'] = df_prev['date']
df_prev_match_completed['home_team'] = df_prev['Team']
df_prev_match_completed['away_team'] = away_teams
df_prev_match_completed['xG'] = df_prev['xG']
df_prev_match_completed['xGA'] = df_prev['xGA']
df_prev_match_completed['npxG'] = df_prev['npxG']
df_prev_match_completed['npxGA'] = df_prev['npxGA']
df_prev_match_completed['deep'] = df_prev['deep']
df_prev_match_completed['deep_allowed'] =df_prev['deep_allowed']
df_prev_match_completed['ppda_coef'] = df_prev['ppda_coef']
df_prev_match_completed['oppda_coef'] = df_prev['oppda_coef']
df_prev_match_completed['fthg'] = df_prev['scored']
df_prev_match_completed['ftag'] = df_prev['missed']

ftrs = []
for index, row in df_prev_match_completed.iterrows():
    if row['fthg'] > row['ftag']:
        ftr = 'H'
    elif row['fthg'] == row['ftag']:
        ftr = 'D'
    elif row['fthg'] < row['ftag']:
        ftr = 'A'
    ftrs.append(ftr)
df_prev_match_completed['ftr'] = ftrs


df_prev_match_completed.to_csv('Data/previous_matches_complete.csv')

df_prev_match_completed = pd.read_csv('Data/previous_matches_complete.csv')


# CALCULATE AVERAGES
def team_averages(df, at_home, match_date):
    # g_f, g_a, xg_f, xg_a, npxg_f, npxg_a, deep_f, deep_a, ppda_f, ppda_a
    if at_home == True:
        adj = 0
    else:
        adj = 1

    g_f = 0.0
    g_a = 0.0
    xG_f = 0.0
    xG_a = 0.0
    npxG_f = 0.0
    npxG_a = 0.0
    deep_f = 0.0
    deep_a = 0.0
    ppda_f = 0.0
    ppda_a = 0.0
    time_wt = 0.0

    for index, row in df.iterrows():
        delta = (match_date - row['date']).days
        this_exp = np.exp(-0.007 * delta)
        time_wt += this_exp
        g_f += (this_exp * row[12+adj])
        g_a += (this_exp * row[13-adj])
        xG_f += (this_exp * row[4+adj])
        xG_a += (this_exp * row[5-adj])
        npxG_f += (this_exp * row[6+adj])
        npxG_a += (this_exp * row[7-adj])
        deep_f += (this_exp * row[8+adj])
        deep_a += (this_exp * row[9-adj])
        ppda_f += (this_exp * row[10+adj])
        ppda_a += (this_exp * row[11-adj])

    g_f /= time_wt
    g_a /= time_wt
    xG_f /= time_wt
    xG_a /= time_wt
    npxG_f /= time_wt
    npxG_a /= time_wt
    deep_f /= time_wt
    deep_a /= time_wt
    ppda_f /= time_wt
    ppda_a /= time_wt

    return [g_f, g_a, xG_f, xG_a, npxG_f, npxG_a, deep_f, deep_a, ppda_f, ppda_a]


def get_all_averages(df, already_played, earliest_date):
    # columns for new dataframe
    leagues = []
    match_dts = []
    home_teams = []
    av_hm_g_fs = []
    av_hm_g_as = []
    av_hm_xG_fs = []
    av_hm_xG_as = []
    av_hm_npxG_fs = []
    av_hm_npxG_as = []
    av_hm_deep_fs = []
    av_hm_deep_as = []
    av_hm_ppda_fs = []
    av_hm_ppda_as = []
    away_teams = []
    av_aw_g_fs = []
    av_aw_g_as = []
    av_aw_xG_fs = []
    av_aw_xG_as = []
    av_aw_npxG_fs = []
    av_aw_npxG_as = []
    av_aw_deep_fs = []
    av_aw_deep_as = []
    av_aw_ppda_fs = []
    av_aw_ppda_as = []
    ftrs = []

    min_games = 10
    ct = 0
    for index, row in df.iterrows():
        match_date = row['date']
        if match_date >= earliest_date:
            df_home = df[(df['home_team'] == row['home_team']) & (df['date'] < match_date)]
            df_away = df[(df['away_team'] == row['away_team']) & (df['date'] < match_date)]
            if (len(df_home.index) >= min_games) & (len(df_away.index) >= min_games):
                ct += 1
                if (ct % 200 == 0):
                    print(f'{ct} games processed for averages')
                df_home = df_home.sort_values(by=['date'])
                df_home = df_home.tail(min_games)
                home_team_averages = team_averages(df_home, True, match_date)

                df_away = df_away.sort_values(by=['date'])
                df_away = df_away.tail(min_games)
                away_team_averages = team_averages(df_away, False, match_date)

                # print(df_away)
                # input()
                
                # print(f"{row['Home_team']} = {home_team_averages}")
                # print(f"{row['Away_team']} = {away_team_averages}")
                # input()
                leagues.append(row['league'])
                match_dts.append(match_date)
                home_teams.append(row['home_team'])
                av_hm_g_fs.append(home_team_averages[0])
                av_hm_g_as.append(home_team_averages[1])
                av_hm_xG_fs.append(home_team_averages[2])
                av_hm_xG_as.append(home_team_averages[3])
                av_hm_npxG_fs.append(home_team_averages[4])
                av_hm_npxG_as.append(home_team_averages[5])
                av_hm_deep_fs.append(home_team_averages[6])
                av_hm_deep_as.append(home_team_averages[7])
                av_hm_ppda_fs.append(home_team_averages[8])
                av_hm_ppda_as.append(home_team_averages[9])
                away_teams.append(row['away_team'])
                av_aw_g_fs.append(away_team_averages[0])
                av_aw_g_as.append(away_team_averages[1])
                av_aw_xG_fs.append(away_team_averages[2])
                av_aw_xG_as.append(away_team_averages[3])
                av_aw_npxG_fs.append(away_team_averages[4])
                av_aw_npxG_as.append(away_team_averages[5])
                av_aw_deep_fs.append(away_team_averages[6])
                av_aw_deep_as.append(away_team_averages[7])
                av_aw_ppda_fs.append(away_team_averages[8])
                av_aw_ppda_as.append(away_team_averages[9])
                if already_played == True:
                    ftrs.append(row['ftr'])


    # create dataframe
    df_processed = pd.DataFrame()
    df_processed['league'] = leagues
    df_processed['date'] = match_dts
    df_processed['home_team'] = home_teams
    df_processed['av_hm_g_f'] = av_hm_g_fs
    df_processed['av_hm_g_a'] = av_hm_g_as
    df_processed['av_hm_xG_f'] = av_hm_xG_fs
    df_processed['av_hm_xG_a'] = av_hm_xG_as
    df_processed['av_hm_npxG_f'] = av_hm_npxG_fs
    df_processed['av_hm_npxG_a'] = av_hm_npxG_as
    df_processed['av_hm_deep_f'] = av_hm_deep_fs
    df_processed['av_hm_deep_a'] = av_hm_deep_as
    df_processed['av_hm_ppda_f'] = av_hm_ppda_fs
    df_processed['av_hm_ppda_a'] = av_hm_ppda_as
    df_processed['away_team'] = away_teams
    df_processed['av_aw_g_f'] = av_aw_g_fs
    df_processed['av_aw_g_a'] = av_aw_g_as
    df_processed['av_aw_xG_f'] = av_aw_xG_fs
    df_processed['av_aw_xG_a'] = av_aw_xG_as
    df_processed['av_aw_npxG_f'] = av_aw_npxG_fs
    df_processed['av_aw_npxG_a'] = av_aw_npxG_as
    df_processed['av_aw_deep_f'] = av_aw_deep_fs
    df_processed['av_aw_deep_a'] = av_aw_deep_as
    df_processed['av_aw_ppda_f'] = av_aw_ppda_fs
    df_processed['av_aw_ppda_a'] = av_aw_ppda_as
    df_processed['ftr'] = ftrs

    # df_processed.to_csv('Data/processed_match_data.csv')
    return df_processed


# Feed previous matches
#df_processed_previous_matches = get_all_averages(df_prev_match_completed, True, pd.to_datetime('1970-01-01'))
#df_processed_previous_matches.to_csv('Data/average_prev_matches.csv')
df_processed_previous_matches = pd.read_csv('Data/average_prev_matches.csv')


# Feed fixtures
df_prev_match_completed['date'] = pd.to_datetime(df_prev_match_completed['date'])
df_prev_match_completed_recent = df_prev_match_completed[df_prev_match_completed['date'] > pd.to_datetime('2020-07-01')]
frames = [df_prev_match_completed_recent, df_fx]
df_fx_and_recent = pd.concat(frames)
df_processed_fx = get_all_averages(df_fx_and_recent, False, earliest_fx_date)
df_processed_fx.to_csv('Data/average_fixtures.csv')
    