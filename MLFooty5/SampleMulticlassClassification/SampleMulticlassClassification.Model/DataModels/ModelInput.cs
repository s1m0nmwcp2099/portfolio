//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace SampleMulticlassClassification.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("league"), LoadColumn(0)]
        public string League { get; set; }


        [ColumnName("match_date"), LoadColumn(1)]
        public string Match_date { get; set; }


        [ColumnName("home_team"), LoadColumn(2)]
        public string Home_team { get; set; }


        [ColumnName("away_team"), LoadColumn(3)]
        public string Away_team { get; set; }


        [ColumnName("av_hm_ftG_for"), LoadColumn(4)]
        public float Av_hm_ftG_for { get; set; }


        [ColumnName("av__hm_ftG_con"), LoadColumn(5)]
        public float Av_hm_ftG_con { get; set; }


        [ColumnName("av_hm_htG_for"), LoadColumn(6)]
        public float Av_hm_htG_for { get; set; }


        [ColumnName("av_hm_htG_con"), LoadColumn(7)]
        public float Av_hm_htG_con { get; set; }


        [ColumnName("av_hm_shots_for"), LoadColumn(8)]
        public float Av_hm_shots_for { get; set; }


        [ColumnName("av_hm_shots_con"), LoadColumn(9)]
        public float Av_hm_shots_con { get; set; }


        [ColumnName("av_hm_shots_target_for"), LoadColumn(10)]
        public float Av_hm_shots_target_for { get; set; }


        [ColumnName("av_hm_shots_target_con"), LoadColumn(11)]
        public float Av_hm_shots_target_con { get; set; }


        [ColumnName("av_hm_corners_for"), LoadColumn(12)]
        public float Av_hm_corners_for { get; set; }


        [ColumnName("av_hm_corners_con"), LoadColumn(13)]
        public float Av_hm_corners_con { get; set; }


        [ColumnName("av_hm_yellow_for"), LoadColumn(14)]
        public float Av_hm_yellow_for { get; set; }


        [ColumnName("av_hm_yellow_con"), LoadColumn(15)]
        public float Av_hm_yellow_con { get; set; }


        [ColumnName("av_hm_red_for"), LoadColumn(16)]
        public float Av_hm_red_for { get; set; }


        [ColumnName("av_hm_red_con"), LoadColumn(17)]
        public float Av_hm_red_con { get; set; }


        [ColumnName("av_hm_ft_wins"), LoadColumn(18)]
        public float Av_hm_ft_wins { get; set; }


        [ColumnName("av_hm_ft_draws"), LoadColumn(19)]
        public float Av_hm_ft_draws { get; set; }


        [ColumnName("av_hm_ft_losses"), LoadColumn(20)]
        public float Av_hm_ft_losses { get; set; }


        [ColumnName("av_hm_win_streak"), LoadColumn(21)]
        public float Av_hm_win_streak { get; set; }


        [ColumnName("av_hm_winless_streak"), LoadColumn(22)]
        public float Av_hm_winless_streak { get; set; }


        [ColumnName("av_hm_draw_streak"), LoadColumn(23)]
        public float Av_hm_draw_streak { get; set; }


        [ColumnName("av_hm_drawless_streak"), LoadColumn(24)]
        public float Av_hm_drawless_streak { get; set; }


        [ColumnName("av_hm_loss_streak"), LoadColumn(25)]
        public float Av_hm_loss_streak { get; set; }


        [ColumnName("av_hm_lossless_streak"), LoadColumn(26)]
        public float Av_hm_lossless_streak { get; set; }


        [ColumnName("av_hm_ht_wins"), LoadColumn(27)]
        public float Av_hm_ht_wins { get; set; }


        [ColumnName("av_hm_ht_draws"), LoadColumn(28)]
        public float Av_hm_ht_draws { get; set; }


        [ColumnName("av_hm_ht_losses"), LoadColumn(29)]
        public float Av_hm_ht_losses { get; set; }


        [ColumnName("av_hm_pld"), LoadColumn(30)]
        public float Av_hm_pld { get; set; }


        [ColumnName("av_aw_ftG_for"), LoadColumn(31)]
        public float Av_aw_ftG_for { get; set; }


        [ColumnName("av__aw_ftG_con"), LoadColumn(32)]
        public float Av_aw_ftG_con { get; set; }


        [ColumnName("av_aw_htG_for"), LoadColumn(33)]
        public float Av_aw_htG_for { get; set; }


        [ColumnName("av_aw_htG_con"), LoadColumn(34)]
        public float Av_aw_htG_con { get; set; }


        [ColumnName("av_aw_shots_for"), LoadColumn(35)]
        public float Av_aw_shots_for { get; set; }


        [ColumnName("av_aw_shots_con"), LoadColumn(36)]
        public float Av_aw_shots_con { get; set; }


        [ColumnName("av_aw_shots_target_for"), LoadColumn(37)]
        public float Av_aw_shots_target_for { get; set; }


        [ColumnName("av_aw_shots_target_con"), LoadColumn(38)]
        public float Av_aw_shots_target_con { get; set; }


        [ColumnName("av_aw_corners_for"), LoadColumn(39)]
        public float Av_aw_corners_for { get; set; }


        [ColumnName("av_aw_corners_con"), LoadColumn(40)]
        public float Av_aw_corners_con { get; set; }


        [ColumnName("av_aw_yellow_for"), LoadColumn(41)]
        public float Av_aw_yellow_for { get; set; }


        [ColumnName("av_aw_yellow_con"), LoadColumn(42)]
        public float Av_aw_yellow_con { get; set; }


        [ColumnName("av_aw_red_for"), LoadColumn(43)]
        public float Av_aw_red_for { get; set; }


        [ColumnName("av_aw_red_con"), LoadColumn(44)]
        public float Av_aw_red_con { get; set; }


        [ColumnName("av_aw_ft_wins"), LoadColumn(45)]
        public float Av_aw_ft_wins { get; set; }


        [ColumnName("av_aw_ft_draws"), LoadColumn(46)]
        public float Av_aw_ft_draws { get; set; }


        [ColumnName("av_aw_ft_losses"), LoadColumn(47)]
        public float Av_aw_ft_losses { get; set; }


        [ColumnName("av_aw_win_streak"), LoadColumn(48)]
        public float Av_aw_win_streak { get; set; }


        [ColumnName("av_aw_winless_streak"), LoadColumn(49)]
        public float Av_aw_winless_streak { get; set; }


        [ColumnName("av_aw_draw_streak"), LoadColumn(50)]
        public float Av_aw_draw_streak { get; set; }


        [ColumnName("av_aw_drawless_streak"), LoadColumn(51)]
        public float Av_aw_drawless_streak { get; set; }


        [ColumnName("av_aw_loss_streak"), LoadColumn(52)]
        public float Av_aw_loss_streak { get; set; }


        [ColumnName("av_aw_lossless_streak"), LoadColumn(53)]
        public float Av_aw_lossless_streak { get; set; }


        [ColumnName("av_aw_ht_wins"), LoadColumn(54)]
        public float Av_aw_ht_wins { get; set; }


        [ColumnName("av_aw_ht_draws"), LoadColumn(55)]
        public float Av_aw_ht_draws { get; set; }


        [ColumnName("av_aw_ht_losses"), LoadColumn(56)]
        public float Av_aw_ht_losses { get; set; }


        [ColumnName("av_aw_pld"), LoadColumn(57)]
        public float Av_aw_pld { get; set; }


        [ColumnName("home_odds"), LoadColumn(58)]
        public float Home_odds { get; set; }


        [ColumnName("draw_odds"), LoadColumn(59)]
        public float Draw_odds { get; set; }


        [ColumnName("away_odds"), LoadColumn(60)]
        public float Away_odds { get; set; }


        [ColumnName("ftr"), LoadColumn(61)]
        public string Ftr { get; set; }


    }
}
