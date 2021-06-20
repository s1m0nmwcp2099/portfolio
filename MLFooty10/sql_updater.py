import mysql.connector.connection
import pandas as pd
from datetime import datetime, timedelta
import numpy as np
import requests
import urllib.request


def remove_first_character_space(thisStr):
    if len(thisStr) > 0 and ord(thisStr[0]) == 32:
        thisStr = thisStr[1:]
    return thisStr


def check_string_characters(thisStr):
    thisStr = str(thisStr)
    for i in range (0, len(thisStr)):
        if ord(thisStr[i]) == 65533:
            thisStr = thisStr.replace(thisStr[i], 'o')
    return thisStr


def reduce_header(hdr):
    if hdr == 'Country':
        hdr = 'Div'
    elif hdr == 'Home' or hdr == 'HT':
        hdr = 'HomeTeam'
    elif hdr == 'Away' or hdr == 'AT':
        hdr = 'AwayTeam'
    elif hdr == 'HG':
        hdr = 'FTHG'
    elif hdr == 'AG':
        hdr = 'FTAG'
    elif hdr == 'PH':
        hdr = 'PSH'
    elif hdr == 'PD':
        hdr = 'PSD'
    elif hdr == 'PA':
        hdr = 'PSA'
    elif hdr == 'Res':
        hdr = 'FTR'
    return hdr


def sqlise_csv_header_line(thisLine):
    thisLine = thisLine.replace('Div', 'ThisDiv')
    thisLine = thisLine.replace('>', 'over')
    thisLine = thisLine.replace('<', 'under')
    thisLine = thisLine.replace('2.5', 'TwoPtFive')
    thisLine = thisLine.replace('365', 'Stk')
    thisLine = thisLine.replace('1X2', 'ResTot')
    thisLine = thisLine.replace('AS', 'AWS')
    return thisLine


def sqlise_csv_variable_line(thisLine):
    thisLine = thisLine.replace('\"', '')
    thisLine = thisLine.replace('INT', 'INT-')
    thisLine = thisLine.replace('TIME', 'TIME-')
    thisLine = thisLine.replace('DATE', 'DATE-')
    thisLine = thisLine.replace('CHAR', 'CHAR-')
    thisLine = thisLine.replace('),', ')-')
    return thisLine


def sqlise_date(dt_str):
    dt_str = dt_str.replace(' ', '')
    dt = datetime.strptime(dt_str, '%d/%m/%Y')
    # new_dt_str = str(dt.year)+'-'+str(dt.month)+'-'+str(dt.day)
    new_dt_str = dt.strftime('%Y-%m-%d')
    # print(new_dt_str)
    return new_dt_str


def remove_punctuation(thisStr):
    thisStr = str(thisStr)
    thisStr = thisStr.replace("'", "")
    thisStr = thisStr.replace(',', '')
    thisStr = thisStr.replace('.', '')
    thisStr = thisStr.replace('-', '')
    return thisStr


def download_and_write_to_mysql(this_url, is_main_euro, league, season):
    conn_str = mysql.connector.connect(
        host = 'localhost',
        user = 'simon',
        password = 'chainsaw',
        database = 'football'
    )

    # is_main_euro: True = main European leagues; False = rest of world leagues
    this_file = 'this_lg_file.csv'
    
    if (is_main_euro == True):
        if (season == '2021'):
            proceed = True
        else:
            proceed = False
    else:
        proceed = True

        
    if proceed == True:
        if is_main_euro == True:
            print(f'Fetching data for league {league}, season {season}')
        else:
            print(f'Fetching data for league {league}')
        urllib.request.urlretrieve(this_url, this_file)
        with open (this_file, 'r', errors='ignore') as s:
            lg_df = pd.read_csv(s)
        #lg_df = pd.read_csv(this_url)
        
        #get headers for sql table
        lg_df_hdrs = lg_df.columns.values.tolist()
        print (type(lg_df_hdrs))
        print(type(lg_df_hdrs[0]))
        #input()

        sql_hdrs_line = ''

        for k in range(0, len(lg_df_hdrs)):
            lg_df_hdrs[k] = reduce_header(lg_df_hdrs[k])
            if k > 0:
                sql_hdrs_line += ','
            sql_hdrs_line += sqlise_csv_header_line(lg_df_hdrs[k])

        sql_hdrs = sql_hdrs_line.split(',')

        print(lg_df_hdrs)
        print(sql_hdrs)
        # input()

        #clean dataframe
        lg_df.iloc[:,sql_hdrs.index('ThisDiv')] = lg_df.iloc[:,sql_hdrs.index('ThisDiv')].apply(remove_first_character_space)
        # lg_df['ThisDiv'] = lg_df['ThisDiv'].apply(remove_first_character_space)
        lg_df.iloc[:,sql_hdrs.index('Date')] = lg_df.iloc[:,sql_hdrs.index('Date')].apply(remove_first_character_space)
        lg_df.iloc[:,sql_hdrs.index('Date')] = lg_df.iloc[:,sql_hdrs.index('Date')].apply(sqlise_date)
        # lg_df.iloc[:,sql_hdrs.index(['HomeTeam', 'AwayTeam', 'Referee'])] = lg_df.iloc[:,sql_hdrs.index(['HomeTeam', 'AwayTeam', 'Referee'])].apply(check_string_characters, remove_punctuation)
        lg_df.iloc[:, sql_hdrs.index('HomeTeam')] = lg_df.iloc[:, sql_hdrs.index('HomeTeam')].apply(check_string_characters)
        lg_df.iloc[:, sql_hdrs.index('HomeTeam')] = lg_df.iloc[:, sql_hdrs.index('HomeTeam')].apply(remove_punctuation)
        lg_df.iloc[:, sql_hdrs.index('AwayTeam')] = lg_df.iloc[:, sql_hdrs.index('AwayTeam')].apply(check_string_characters)
        lg_df.iloc[:, sql_hdrs.index('AwayTeam')] = lg_df.iloc[:, sql_hdrs.index('AwayTeam')].apply(remove_punctuation)
        if 'Referee' in sql_hdrs:
            lg_df.iloc[:, sql_hdrs.index('Referee')] = lg_df.iloc[:, sql_hdrs.index('Referee')].apply(check_string_characters)
            lg_df.iloc[:, sql_hdrs.index('Referee')] = lg_df.iloc[:, sql_hdrs.index('Referee')].apply(remove_punctuation)
        lg_df = lg_df.fillna('NULL')

        for rw in range (0, len(lg_df.index)):
            # filter dates after last update
            this_match_date = datetime.strptime(lg_df.iloc[rw, sql_hdrs.index('Date')], '%Y-%m-%d')

            if this_match_date > (last_update_date - timedelta(days=7)):
                sql_query_start = 'REPLACE INTO football_data_complete ('
                sql_query_end = ' VALUES ('
                for cl in range (0, len(sql_hdrs)):
                    if cl > 0:
                        sql_query_start += ', '
                        sql_query_end += ', '
                    sql_query_start += str(sql_hdrs[cl])
                    if sql_hdrs[cl] in need_quotes and str(lg_df.iloc[rw, cl]) != 'NULL':
                        sql_query_end += "'"
                    sql_query_end += str(lg_df.iloc[rw, cl])
                    if sql_hdrs[cl] in need_quotes and str(lg_df.iloc[rw, cl]) != 'NULL':
                        sql_query_end += "'"
                sql_query_start += ')'
                sql_query_end += ');'
                sql_query = sql_query_start + sql_query_end
                print(sql_query)
                # input()
                my_cursor = conn_str.cursor()
                my_cursor.execute(sql_query)
                conn_str.commit()
    conn_str.close()


# corresponds to start of main(line 112) for BigFootySql/Program.cs
update_date_file = '../BigFootySql/when.txt' # change this when moving project over to githubrepo

last_update_date = datetime(1970,2,1)
with open (update_date_file, 'r') as dt_file:
    last_update_date = dt_file.read().replace('\n', '')
    last_update_date = datetime.strptime(last_update_date, '%d/%m/%Y %H:%M:%S')

seasons = ['9394', '9495', '9596', '9697', '9798', '9899', '9900', '0001', '0102', '0203', '0304', '0405', '0506', '0607', '0708',
       '0809', '0910', '1011', '1112', '1213', '1314', '1415', '1516', '1617', '1718', '1819', '1920', '2021']
# conditions for above list can be found from line 166 of BigFootySql/Program.cs
# seasons = ['1920', '2021']
leagues = ['B1', 'D1', 'D2', 'E0', 'E1', 'E2', 'E3', 'EC', 'F1', 'F2', 'G1', 'I1', 'I2', 'N1', 'P1', 'SC0', 'SC1', 'SC2', 'SC3',
        'SP1', 'SP2', 'T1']
extra_leagues = ['ARG', 'AUT', 'BRA', 'CHN', 'DNK', 'FIN', 'IRL', 'JPN', 'MEX', 'NOR', 'POL', 'ROU', 'RUS', 'SWE', 'SWZ', 'USA']


is_data_main_lgs = np.zeros([len(leagues), len(seasons)], dtype=bool)
# this will need amending for including earlier seasons
for j in range(0, len(seasons)):
    if j >= (len(seasons) - 2): # this line is to save time
        if j >= 2:  
            is_data_main_lgs[leagues.index('B1'), j] = True
        is_data_main_lgs[leagues.index('D1'), j] = True
        is_data_main_lgs[leagues.index('D2'), j] = True
        is_data_main_lgs[leagues.index('E0'), j] = True
        is_data_main_lgs[leagues.index('E1'), j] = True
        is_data_main_lgs[leagues.index('E2'), j] = True
        is_data_main_lgs[leagues.index('E3'), j] = True
        if j >= 12:
            is_data_main_lgs[leagues.index('EC'), j] = True
        is_data_main_lgs[leagues.index('F1'), j] = True
        if j >= 3:
            is_data_main_lgs[leagues.index('F2'), j] = True
            is_data_main_lgs[leagues.index('SP2'), j] = True
        if j >= 1:
            is_data_main_lgs[leagues.index('G1'), j] = True
            is_data_main_lgs[leagues.index('P1'), j] = True
            is_data_main_lgs[leagues.index('SC0'), j] = True
            is_data_main_lgs[leagues.index('SC1'), j] = True
            is_data_main_lgs[leagues.index('T1'), j] = True
        is_data_main_lgs[leagues.index('I1'), j] = True
        if j >= 4:
            is_data_main_lgs[leagues.index('I2'), j] = True
            is_data_main_lgs[leagues.index('SC2'), j] = True
            is_data_main_lgs[leagues.index('SC3'), j] = True
        is_data_main_lgs[leagues.index('N1'), j] = True
        is_data_main_lgs[leagues.index('SP1'), j] = True
    



need_quotes = ['ThisDiv', 'Date', 'HomeTeam', 'AwayTeam', 'FTR', 'HTR', 'Time', 'Referee', 'LB', 'HT', 'AT', 'League', 'Season']


# main leagues
for i in range(0, len(leagues)):
    for j in range(0, len(seasons)):
        if is_data_main_lgs[i,j] == True:
            this_url = f'http://www.football-data.co.uk/mmz4281/{seasons[j]}/{leagues[i]}.csv'
            download_and_write_to_mysql(this_url, True, leagues[i], seasons[j])


# extra leagues
for i in range(0, len(extra_leagues)):
    # string leagueUrl = $"https://www.football-data.co.uk/new/{ExtraLeagues[i]}.csv";
    this_url = f'http://www.football-data.co.uk/new/{extra_leagues[i]}.csv'
    download_and_write_to_mysql(this_url, False, extra_leagues[i], 'null')


dt_now = datetime.now()
dt_now = datetime.strftime(dt_now, '%d/%m/%Y %H:%M:%S')
with open (update_date_file, 'w') as dt_file:
    dt_file.write(dt_now)

# back to line 223 of program.cs for extra leagues