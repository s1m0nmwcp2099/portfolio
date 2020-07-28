import requests
import pandas as pd
import numpy as np
from tqdm import tqdm
import matplotlib.pyplot as plt
from sklearn.linear_model import LinearRegression

base_url = "https://raw.githubusercontent.com/statsbomb/open-data/master/data/"
comp_url = base_url + "matches/{}/{}.json"
match_url = base_url + "events/{}.json"

def parse_data(competition_id, season_id):
    matches = requests.get(url = comp_url.format(competition_id, season_id)).json()
    match_ids = [m['match_id'] for m in matches]
    
    all_events = []
    for match_id in tqdm(match_ids):

        events = requests.get(url = match_url.format(match_id)).json()

        shots = [x for x in events if x['type']['name'] == "Shot"]
        #passes = [x for x in events if x['type']['name'] == "Pass"]
        
        for s in shots:
            attributes = {
                """
                "match_id": match_id,
                "team": s["possession_team"]["name"],
                "player": s['player']['name'],
                "x": s['location'][0],
                "y": s['location'][1],
                "outcome": s['shot']['outcome']['name'],
                """
                "x": s['location'][0],
                "y": s['location'][1],
                "head": 1 if s['shot']['body_part']['name'] == "Head" else 0,
                "phase": s['shot']['type']['name'],
                "outcome": 1 if s['shot']['outcome']['name'] == "Goal" else 0,
                "statsbomb_xg": s['shot']['statsbomb_xg']
            }
            all_events.append(attributes)
        """
        for a in passes:
            attributes = {
                "player_id": a['player']['id'],
                "outcome": 0 if 'outcome' in a['pass'].keys() else 1
            }
            all_events.append(attributes)
            """
    return pd.DataFrame(all_events)

competition_id = 43
season_id = 3
df = parse_data(competition_id, season_id)
"""
df.head(15)
total_passes = df.groupby('player_id')['outcome'].sum()
percentage = df.groupby('player_id')['outcome'].mean()
plt.scatter(total_passes, percentage, alpha = 0.8)
plt.show()

model = LinearRegression()
fit = model.fit([[x] for x in total_passes], percentage)
print("Coefficients: {}".format(fit.coef_))
print("Intercept: {}".format(fit.intercept_))

xfit = [0, 500]
yfit = model.predict([[x] for x in xfit])
plt.scatter(total_passes, percentage, alpha = 0.3)
plt.plot(xfit, yfit, 'r')
plt.show()
"""
def distance_to_goal(origin):
    dest = np.array([120.,40.])
    return np.sqrt(np.sum(origin - dest) ** 2)

def goal_angle(origin):
    p0 = np.array((120., 36.)) #left post
    p1 = np.array(origin, dtype=np.float)
    p2 = np.array((120., 44.)) #right post

    v0 = p0 - p1
    v1 = p2 - p1

    angle = np.abs(np.math.atan2(np.linalg.det([v0, v1]), np.dot(v0, v1)))
    return angle

df['distance_to_goal'] = df.apply(lambda row: distance_to_goal(row[['x', 'y']]), axis=1)
df['goal_angle'] = df.apply(lambda r: goal_angle(r[['x', 'y']]), axis=1)

shots = df[~df['phase'].isin(['Free Kick', 'Penalty'])]

from sklearn.linear_model import LogisticRegression
model = LogisticRegression()

features = shots[['distance_to_goal', 'goal_angle', 'head']]
labels = shots['outcome']

fit = model.fit(features, labels)

predictions = model.predict_proba(features)[:, 1]

plt.plot(sorted(predictions))
plt.show()

plt.plot(sorted(shots['statsbomb_xg']))
plt.show()