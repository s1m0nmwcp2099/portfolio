// Learn more about F# at http://fsharp.org

open System
open MySql.Data.MySqlClient
open Microsoft.Data.Analysis

let get_sql_data() =
    let thisDiv = StringDataFrameColumn("ThisDiv", int64 0)
    let matchDates = PrimitiveDataFrameColumn<DateTime>("MatchDates", int64 0)
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
        matchDates.Append(Nullable<DateTime>(rdr.GetDateTime(1)))
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

let fetch_data() =
    //Create dataframe columns
    let thisDiv = StringDataFrameColumn("ThisDiv", int64 0)
    let matchDates = PrimitiveDataFrameColumn<DateTime>("MatchDates", int64 0)
    let homeTeams = StringDataFrameColumn("HomeTeams", int64 0)
    let awayTeams = StringDataFrameColumn("AwayTeams", int64 0)
    let fthg = PrimitiveDataFrameColumn<int>("Fthg", int64 0)   //home goals
    let ftag = PrimitiveDataFrameColumn<int>("Ftag", int64 0)   //away goals
    let ftr = StringDataFrameColumn("Ftr", int64 0)             //result
    let hs = PrimitiveDataFrameColumn<int>("Hs", int64 0)       //home shots
    let aws = PrimitiveDataFrameColumn<int>("Aws", int64 0)     //away shots
    let hst = PrimitiveDataFrameColumn<int>("Hst", int64 0)     //home shots on target
    let awst = PrimitiveDataFrameColumn<int>("Awst", int64 0)

    //fetch match data from mysql
    let sql = "SELECT COUNT(*) FROM football_data_complete WHERE FTHG > -1 AND FTAG > -1 AND HS > -1 AND AwS > -1 AND HST > -1 AND AwST > -1;"
    let conn = MySqlConnection(string: connStr)
    conn

[<EntryPoint>]
let main argv =
    
    0 // return an integer exit code
