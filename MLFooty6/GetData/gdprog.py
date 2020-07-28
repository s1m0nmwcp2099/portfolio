import mysql.connector
import numpy as np
import pandas as pd
import math
from datetime import date
from sklearn import preprocessing

print("Cooey")

mydb = mysql.connector.connect(
    host = "localhost",
    user = "simon",
    password = "chainsaw",
    database = "football"
)
#get all historic results from mysql and put into dataframe
mycursor = mydb.cursor()
mycursor.execute("SELECT * FROM results_and_odds")
historic_results = pd.DataFrame(mycursor.fetchall())
#mydb.close()

def convert_from_sql(sql_date):
    parts = str(sql_date).split('-')
    py_date = date(int(parts[0]), int(parts[1]), int(parts[2]))
    return py_date

def get_team_data(team, at_hm, sql_dt):
    if (at_hm == True):
        adj = 0
        tm = "home_team"
    else:
        adj = 1
        tm = "away_team"
    
    mycursor = mydb.cursor()
    mycursor.execute("SELECT * FROM results_and_odds WHERE {} = '{}' AND date < '{}' ORDER BY date DESC LIMIT 10".format(tm, team, sql_dt) )
    recent_results = pd.DataFrame(mycursor.fetchall())
    #print (recent_results)

    if len(recent_results) < 10:
        return False
    else:
        av_gls_sco = 0.0
        av_gls_con = 0.0
        av_pld = 0.0
        av_wins = 0.0
        av_draws = 0.0
        av_losses = 0.0

        winning_streak = True
        winning_ct = 0.0
        winless_streak = True
        winless_ct = 0.0
        drawing_streak = True
        drawing_ct = 0.0
        drawless_streak = True
        drawless_ct = 0.0
        losing_streak = True
        losing_ct = 0.0
        lossless_streak = True
        lossless_ct = 0.0
        
        for i in range(0,10):
            delta = (convert_from_sql(sql_dt) - convert_from_sql(recent_results.iloc[i, 1])).days
            my_exp = math.exp(delta * -0.007)
            #my_exp = 1
            goals_scored = recent_results.iloc[i, 4 + adj]
            goals_conceded = recent_results.iloc[i, 5 - adj]
            av_gls_sco += (goals_scored * my_exp)
            av_gls_con += (goals_conceded * my_exp)
            av_pld += my_exp
            if goals_scored > goals_conceded: #win
                av_wins += my_exp
                if winning_streak == True:
                    winning_ct += 1
                if drawless_streak == True:
                    drawless_ct += 1
                if lossless_streak == True:
                    lossless_ct += 1
                winless_streak = False
                drawing_streak = False
                losing_streak = False
            elif goals_scored == goals_conceded: #draw
                av_draws += my_exp
                if winless_streak == True:
                    winless_ct += 1
                if drawing_streak == True:
                    drawing_ct += 1
                if lossless_streak == True:
                    lossless_ct += 1
                winning_streak = False
                drawless_streak = False
                losing_streak = False
            else:                           #loss
                av_losses += my_exp
                if winless_streak == True:
                    winless_ct += 1
                if drawless_streak == True:
                    drawless_ct += 1
                if losing_streak == True:
                    losing_ct += 1
                winning_streak = False
                drawing_streak = False
                lossless_streak = False

        av_gls_sco /= av_pld
        av_gls_con /= av_pld
        av_wins /= av_pld
        av_draws /= av_pld
        av_losses /= av_pld

        winning_ct /= 10
        winless_ct /= 10
        drawing_ct /=10
        drawless_ct /= 10
        losing_ct /= 10
        lossless_ct /= 10
        
        return ["{}".format(team), str(av_gls_sco), str(av_gls_con), str(av_wins), str(av_draws), str(av_losses), str(winning_ct), str(winless_ct), str(drawing_ct), str(drawless_ct), str(losing_ct), str(lossless_ct)]

def get_opposition_data(team, at_hm, sql_dt, historic_teams_data):
    ct = 0
    adj = 0
    if at_hm == True:
        adj = 12
    opp_goals_scored = 0.0
    opp_goals_conceded = 0.0
    opp_wins = 0.0
    opp_draws = 0.0
    opp_losses = 0.0
    opp_winning_run = 0.0
    opp_winless_run = 0.0
    opp_drawing_run = 0.0
    opp_drawless_run = 0.0
    opp_losing_run = 0.0
    opp_lossless_run = 0.0
    opp_av_pld = 0.0
    for i in range (1, len(historic_teams_data)):
        if ct < 10:
            prev_line = historic_teams_data[len(historic_teams_data) - 1 - i]
            prev_line_parts = prev_line.split(',')
            if prev_line_parts[2 + adj] == team:
                prev_sql_dt = prev_line_parts[1]
                delta = (convert_from_sql(sql_dt) - convert_from_sql(prev_sql_dt)).days
                my_exp = math.exp(delta * -0.007)
                opp_goals_scored += (my_exp * float(prev_line_parts[3 + adj]))
                opp_goals_conceded += (my_exp * float(prev_line_parts[4 + adj]))
                opp_wins += (my_exp * float(prev_line_parts[5 +adj]))
                opp_draws += (my_exp * float(prev_line_parts[6 + adj]))
                opp_losses += (my_exp * float(prev_line_parts[7 + adj]))
                opp_winning_run += (my_exp * float(prev_line_parts[8 + adj]))
                opp_winless_run += (my_exp * float(prev_line_parts[9 + adj]))
                opp_drawing_run += (my_exp * float(prev_line_parts[10 + adj]))
                opp_drawless_run += (my_exp * float(prev_line_parts[11 + adj]))
                opp_losing_run += (my_exp * float(prev_line_parts[12 + adj]))
                opp_lossless_run += (my_exp * float(prev_line_parts[13 + adj]))
                opp_av_pld += my_exp
                ct += 1
        else:
            break
    if ct < 10:
        return False
    else:
        opp_goals_scored /= opp_av_pld
        opp_goals_conceded /= opp_av_pld
        opp_wins /= opp_av_pld
        opp_draws /= opp_av_pld
        opp_losses /= opp_av_pld
        opp_winning_run /= opp_av_pld
        opp_winless_run /= opp_av_pld
        opp_drawing_run /= opp_av_pld
        opp_drawless_run /= opp_av_pld
        opp_losing_run /= opp_av_pld
        opp_lossless_run /= opp_av_pld
        return [str(opp_goals_scored), str(opp_goals_conceded), str(opp_wins), str(opp_draws), str(opp_losses), str(opp_winning_run), str(opp_winless_run), str(opp_drawing_run), str(opp_drawless_run), str(opp_losing_run), str(opp_lossless_run)]

data = get_team_data('Nottm Forest', False, '2020-07-12')
print (data)


historic_teams_data = []
historic_teams_and_opp_data = []
#historic_teams_and_opp_data.append("league,match_date,home_team,hm_gls_for,hm_gls_con,hm_wins,hm_draws,hm_losses,hm_win_strk,hm_winless_strk,hm_draw_strk,hm_drawless_strk,hm_loss_strk,hm_lossless_strk,away_team,aw_gls_for,aw_gls_con,aw_wins,aw_draws,aw_losses,aw_win_strk,aw_winless_strk,aw_draw_strk,aw_drawless_strk,aw_loss_strk,aw_lossless_strk,hm_odds,dr_odds,aw_odds,result,hm_opp_gls_for,hm_opp_gls_con,hm_opp_wins,hm_opp_draws,hm_opp_losses,hm_opp_win_strk,hm_opp_winless_strk,hm_opp_draw_strk,hm_opp_drawless_strk,hm_opp_loss_strk,hm_opp_lossless_strk,aw_opp_gls_for,aw_opp_gls_con,aw_opp_wins,aw_opp_draws,aw_opp_losses,aw_opp_win_strk,aw_opp_winless_strk,aw_opp_draw_strk,aw_opp_drawless_strk,aw_opp_loss_strk,aw_opp_lossless_strk")

#iterate through historic_results
for i in range(0, len(historic_results)):
    home_team = historic_results.iloc[i, 2]
    away_team = historic_results.iloc[i, 3]
    this_sql_dt = historic_results.iloc[i, 1]
    #get home team data
    home_data = get_team_data(home_team, True, this_sql_dt)
    #get away team data
    away_data = get_team_data(away_team, False, this_sql_dt)
    
    home_goals = historic_results.iloc[i, 4]
    away_goals = historic_results.iloc[i, 5]
    if home_goals > away_goals:
        result = "H"
    elif home_goals < away_goals:
        result = "A"
    else:
        result = "D"

    if home_data != False and away_data != False:
        home_line = ','.join(home_data)
        away_line = ','.join(away_data)
        new_line = "{},{},".format(historic_results.iloc[i, 0], historic_results.iloc[i, 1]) + home_line + "," + away_line + ",{},{},{},{}".format(str(historic_results.iloc[i, 6]), str(historic_results.iloc[i, 7]), str(historic_results.iloc[i, 8]), result)
        print (new_line)
        #input("")
        historic_teams_data.append(new_line)

        #get opposition data
        home_opp_data = get_opposition_data(home_team, True, this_sql_dt, historic_teams_data)
        away_opp_data = get_opposition_data(away_team, False, this_sql_dt, historic_teams_data)

        if home_opp_data != False and away_opp_data != False:
            new_line += ("," + ','.join(home_opp_data) + "," + ','.join(away_opp_data))
            print (new_line)
            historic_teams_and_opp_data.append(new_line)
        else:
            print ("Insufficient opposition data")

whole_df_no_headers = pd.DataFrame([sub.split(',') for sub in historic_teams_and_opp_data])
whole_df_no_headers.to_csv('../Data/historical_match_data_without_headers.csv')
#put list into dataframe
whole_df = pd.DataFrame([sub.split(',') for sub in historic_teams_and_opp_data], columns = ['league', 'match_date', 'home_team', 'hm_gls_for', 'hm_gls_con', 'hm_wins', 'hm_draws', 'hm_losses', 'hm_win_strk', 'hm_winless_strk', 'hm_draw_strk', 'hm_drawless_strk', 'hm_loss_strk', 'hm_lossless_strk', 'away_team', 'aw_gls_for', 'aw_gls_con', 'aw_wins', 'aw_draws', 'aw_losses', 'aw_win_strk', 'aw_winless_strk', 'aw_draw_strk', 'aw_drawless_strk', 'aw_loss_strk', 'aw_lossless_strk', 'hm_odds', 'dr_odds', 'aw_odds', 'result', 'hm_opp_gls_for', 'hm_opp_gls_con', 'hm_opp_wins', 'hm_opp_draws', 'hm_opp_losses', 'hm_opp_win_strk', 'hm_opp_winless_strk', 'hm_opp_draw_strk', 'hm_opp_drawless_strk', 'hm_opp_loss_strk', 'hm_opp_lossless_strk', 'aw_opp_gls_for', 'aw_opp_gls_con', 'aw_opp_wins', 'aw_opp_draws', 'aw_opp_losses', 'aw_opp_win_strk', 'aw_opp_winless_strk', 'aw_opp_draw_strk', 'aw_opp_drawless_strk', 'aw_opp_loss_strk', 'aw_opp_lossless_strk'])
whole_df.to_csv('../Data/historical_match_data_with_headers.csv')
#normalize
x = whole_df[['hm_gls_for', 'hm_gls_con', 'aw_gls_for', 'aw_gls_con', 'hm_opp_gls_for', 'hm_opp_gls_con', 'aw_opp_gls_for', 'aw_opp_gls_con', 'hm_opp_win_strk', 'hm_opp_winless_strk', 'hm_opp_draw_strk', 'hm_opp_drawless_strk', 'hm_opp_loss_strk', 'hm_opp_lossless_strk', 'aw_opp_win_strk', 'aw_opp_winless_strk', 'aw_opp_draw_strk', 'aw_opp_drawless_strk', 'aw_opp_loss_strk', 'aw_opp_lossless_strk', 'hm_odds', 'dr_odds', 'aw_odds']]
"""
min_max_scaler = preprocessing.MinMaxScaler()
x_scaled = min_max_scaler.fit_transform(x)
x_scaled.to_csv('../Data/historical_match_data_with_headers_normalized.csv')
"""
