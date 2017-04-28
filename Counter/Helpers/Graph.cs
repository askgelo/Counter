using Counter.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;

namespace Counter.Helpers
{
    public static class GraphHelper
    {
        public static MvcHtmlString CreateChart(this HtmlHelper html, Graph graph)
        {
            var chart = new Chart()
            {
                Width = 900,
                Height = 500,
                RenderType = RenderType.ImageTag,
                ImageType = ChartImageType.Jpeg,
                ImageLocation = $"TempImages\\ChartPic{Guid.NewGuid()}.jpg",
                BackColor = Color.Gray,
                BackSecondaryColor = Color.WhiteSmoke,
                BackGradientStyle = GradientStyle.DiagonalRight,
                BorderlineDashStyle = ChartDashStyle.Solid,
                BorderlineColor = Color.Gray
            };

            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            var chartArea = new ChartArea("area1");

            chartArea.AxisY.LabelStyle.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorTickMark.Enabled = false;

            chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None;
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Months;
            chartArea.AxisX.LabelStyle.Format = "MMMM yyyy";

            chartArea.BackColor = Color.Bisque;


            chart.ChartAreas.Add(chartArea);

            foreach (var counter in graph.Counters)
            {
                var series = new Series(counter.Counter)
                {
                    ChartType = SeriesChartType.StepLine,
                    //Color = Color.Black,
                    ChartArea = "area1",
                    XValueType = ChartValueType.DateTime,
                    BorderWidth = 2
                };
                foreach (var value in counter.Values)
                {
                    series.Points.AddXY(value.Date.DateTime, value.Percent);
                }
                chart.Series.Add(series);
                chart.Legends.Add(new Legend(counter.Counter));
            }

            chartArea.AxisX.Interval = Math.Round((double)(graph.LengthPeriodInMonths / 10), 0) + 1;

            // Render chart control
            chart.SaveImage(AppDomain.CurrentDomain.BaseDirectory + chart.ImageLocation);
            var tag = new TagBuilder("img");
            tag.Attributes.Add("src", "/" + chart.ImageLocation.Replace("\\", "/"));
            //tag.Attributes.Add("style", $"height:{chart.Height};width:{chart.Width};border-width:0px;");

            return new MvcHtmlString(tag.ToString());
        }
    }
}