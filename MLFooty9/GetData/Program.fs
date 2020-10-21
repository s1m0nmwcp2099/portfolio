// Learn more about F# at http://fsharp.org

open System
open MySql.Data.MySqlClient
open Microsoft.Data.Analysis

let XperYcorrect0 x y =
    if y <> 0f then
        x / y
    else
        1f

let get_sql_data() =
    let thisDiv = StringDataFrameColumn("ThisDiv", int64 0)
    //let matchDates = PrimitiveDataFrameColumn<DateTime>("MatchDates", int64 0)
    let matchDates = StringDataFrameColumn("MatchDates", int64 0)
    let homeTeams = StringDataFrameColumn("HomeTeams", int64 0)
    let awayTeams = StringDataFrameColumn("AwayTeams", int64 0)
    let fthg = PrimitiveDataFrameColumn<int>("Fthg", int64 0)   //home goals
    let ftag = PrimitiveDataFrameColumn<int>("Ftag", int64 0)   //away goals
    let ftr = StringDataFrameColumn("Ftr", int64 0)             //result
    let hs = PrimitiveDataFrameColumn<int>("Hs", int64 0)       //home shots
    let aws = PrimitiveDataFrameColumn<int>("Aws", int64 0)     //away shots
    let hst = PrimitiveDataFrameColumn<int>("Hst", int64 0)     //home shots on target
    let awst = PrimitiveDataFrameColumn<int>("Awst", int64 0)
    
    let connStr = "server = localhost; user = simon; database = football; password = chainsaw"
    let dbConn: MySqlConnection = new MySqlConnection(connStr)
    let sql = "SELECT ThisDiv, Date, HomeTeam, AwayTeam, FTHG, FTAG, FTR, HS, AwS, HST, AwST FROM football_data_complete WHERE FTHG > -1 AND FTAG > -1 AND HS > -1 AND AwS > -1 AND HST > -1 AND AwST > -1;"
    dbConn.Open()
    let cmd: MySqlCommand = new MySqlCommand(sql, dbConn)
    let rdr: MySqlDataReader = cmd.ExecuteReader()
    while rdr.Read() do
        thisDiv.Append(rdr.GetString(0))
        //matchDates.Append(Nullable<DateTime>(rdr.GetDateTime(1)))
        matchDates.Append(rdr.GetString(1))
        homeTeams.Append(rdr.GetString(2))
        awayTeams.Append(rdr.GetString(3))
        fthg.Append(Nullable<int>(rdr.GetInt32(4)))
        ftag.Append(Nullable<int>(rdr.GetInt32(5)))
        ftr.Append(rdr.GetString(6))
        hs.Append(Nullable<int>(rdr.GetInt32(7)))
        aws.Append(Nullable<int>(rdr.GetInt32(8)))
        hst.Append(Nullable<int>(rdr.GetInt32(9)))
        awst.Append(Nullable<int>(rdr.GetInt32(10)))
    dbConn.Close()
    
    let cols = [thisDiv :> DataFrameColumn; matchDates :> DataFrameColumn; homeTeams :> DataFrameColumn; awayTeams :> DataFrameColumn; fthg :> DataFrameColumn; ftag :> DataFrameColumn; ftr :> DataFrameColumn; hs :> DataFrameColumn; aws :> DataFrameColumn; hst :> DataFrameColumn; awst :> DataFrameColumn]
    
    let df = DataFrame(cols)

    for matchInd = thisDiv.Length - 1 downto 0 do
        let mutable prevMatchInd = matchInd - 1
        for adj = 0 to 1 do
            let mutable thisCt = 0
            let mutable rpg = [|0f; 0f; 0f|]    //goals per game: 0=scored per game; 1=conceded per game
            let mutable gpg = [|0f; 0f|]        //goals per game: 0=scored per game; 1=conceded per game
            let mutable gpst = [|0f; 0f|]       //goals per shot on target: 0=for; 1= against
            let mutable gps = [|0f; 0f|]        //goals per shot: 0=for; 1=against
            let mutable stpg = [|0f; 0f|]       //shots on target per game
            let mutable stps = [|0f; 0f|]       //shots on target per shot
            let mutable spg = [|0f; 0f|]        //shots per game
            let mutable expPld = 0f

            while thisCt <= 10 && prevMatchInd >= 0 do
                if df.[int64 matchInd, 2 + adj] = df.[int64 prevMatchInd,2 + adj] then
                    let gf = int64 df.[int64 prevMatchInd, 4 + adj]     //goals for in this previous match
                    let ga = int df.[int64 prevMatchInd, 5 - adj]     //goals against
                    let sf = df.[int64 prevMatchInd, 7 + adj]     //shots for
                    let sa = df.[int64 prevMatchInd, 8 - adj]     //shots against
                    let stf = df.[int64 prevMatchInd, 9 + adj]    //shots on target for
                    let sta = df.[int64 prevMatchInd, 10 - adj]
                    let thisDt = DateTime.Parse df.[int64 matchInd, 1]
                    let thatDt = DateTime df.[int64 prevMatchInd, 1]
                    let delta = (thisDt - thatDt).Days
                    let myExp = MathF.Exp(delta, -0.007f)
                    if gf > ga then                 //win
                        rpg.[0] <- rpg.[0] + myExp
                    elif gf = ga then               //draw
                        rpg.[1] <- rpg.[1] + myExp
                    elif gf < ga then               //loss
                        rpg.[2] <- rpg.[2] + myExp
                    gpg.[0] <- gpg.[0] + gf * myExp
                    gpg.[1] <- gpg.[1] + ga * myExp
                    gpst.[0] <- gpst.[0] + XperYcorrect0 gf stf * myExp
                    gpst.[1] <- gpst.[1] + XperYcorrect0 ga sta * myExp
                    gps.[0] <- gps.[0] + XperYcorrect0 gf sf * myExp
                    gps.[1] <- gps.[1] + XperYcorrect0 ga sa * myExp
                    stpg.[0] <- stpg.[0] + stf * myExp
                    stpg.[1] <- stpg.[1] + sta * myExp
                    stps.[0] <- stps.[0] + XperYcorrect0 stf sf * myExp
                    stps.[1] <- stps.[1] + XperYcorrect0 sta sa * myExp
                    spg.[0] <- spg.[0] + sf * myExp
                    spg.[1] <- spg.[1] + sa * myExp
                    expPld <- expPld + myExp


let averages_per_game (atHm: bool) (matchInd: int) (df: DataFrame) =
    let mutable prevMatchInd = matchInd - 1
    let mutable adj = 0
    if atHm = false then
        adj <- 1
    let mutable thisCt = 0
    let mutable rpg = [|0f; 0f; 0f|]    //goals per game: 0=scored per game; 1=conceded per game
    let mutable gpg = [|0f; 0f|]        //goals per game: 0=scored per game; 1=conceded per game
    let mutable gpst = [|0f; 0f|]       //goals per shot on target: 0=for; 1= against
    let mutable gps = [|0f; 0f|]        //goals per shot: 0=for; 1=against
    let mutable stpg = [|0f; 0f|]       //shots on target per game
    let mutable stps = [|0f; 0f|]       //shots on target per shot
    let mutable spg = [|0f; 0f|]        //shots per game
    let mutable expPld = 0f

    while thisCt <= 10 && prevMatchInd >= 0 do
        if df.[int64 matchInd, 2 + adj] = df.[int64 prevMatchInd,2 + adj] then
            let gf = df.[int64 prevMatchInd, 4 + adj]     //goals for in this previous match
            let ga = df.[int64 prevMatchInd, 5 - adj]     //goals against
            let sf = df.[int64 prevMatchInd, 7 + adj]     //shots for
            let sa = df.[int64 prevMatchInd, 8 - adj]     //shots against
            let stf = df.[int64 prevMatchInd, 9 + adj]    //shots on target for
            let sta = df.[int64 prevMatchInd, 10 - adj]
            let thisDt = df.[int64 matchInd, 1]
            let thatDt = df.[int64 prevMatchInd, 1]
            let delta = DateTime thisDt
            let myExp = MathF.Exp(delta, -0.007f)
            if gf > ga then                 //win
                rpg.[0] <- rpg.[0] + myExp
            elif gf = ga then               //draw
                rpg.[1] <- rpg.[1] + myExp
            elif gf < ga then               //loss
                rpg.[2] <- rpg.[2] + myExp
            gpg.[0] <- gpg.[0] + gf * myExp
            gpg.[1] <- gpg.[1] + ga * myExp
            gpst.[0] <- gpst.[0] + XperYcorrect0 gf stf * myExp
            gpst.[1] <- gpst.[1] + XperYcorrect0 ga sta * myExp
            gps.[0] <- gps.[0] + XperYcorrect0 gf sf * myExp
            gps.[1] <- gps.[1] + XperYcorrect0 ga sa * myExp
            stpg.[0] <- stpg.[0] + stf * myExp
            stpg.[1] <- stpg.[1] + sta * myExp
            stps.[0] <- stps.[0] + XperYcorrect0 stf sf * myExp
            stps.[1] <- stps.[1] + XperYcorrect0 sta sa * myExp
            spg.[0] <- spg.[0] + sf * myExp
            spg.[1] <- spg.[1] + sa * myExp
            expPld <- expPld + myExp


            
                
    


[<EntryPoint>]
let main argv =
    
    0 // return an integer exit code
