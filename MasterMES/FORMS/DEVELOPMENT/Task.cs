using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MasterMES.FORMS.DEVELOPMENT
{
    class Task
    {
        public int ID { get; set; }
        public string WorkName { get; set; }
        public string ChargeName { get; set; }
        public string WorkDetails { get; set; }
        public string WorkRemark { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double? Progress2 { get; set; }

        public Task()
        {

        }

        public static List<Task> GetList(DataTable dt)
        {
            List<Task> list = new List<Task>();
            Task task = new Task();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //task.ID = i + 1;
                //task.WorkName = dt.Rows[i]["WorkName"].ToString();
                //task.ChargeName = dt.Rows[i]["ChargeName"].ToString();
                //task.WorkDetails = dt.Rows[i]["WorkDetails"].ToString();
                //task.WorkRemark = dt.Rows[i]["WorkRemark"].ToString();
                //task.StartDate = Convert.ToDateTime(dt.Rows[i]["StartDate"].ToString());
                //task.EndDate = Convert.ToDateTime(dt.Rows[i]["EndDate"].ToString());
                //task.Progress = Convert.ToDouble(dt.Rows[i]["Progress"].ToString());

                list.Add(GetTask(i + 1,
                         dt.Rows[i]["WorkName"].ToString(),
                         dt.Rows[i]["ChargeName"].ToString(),
                         dt.Rows[i]["WorkDetails"].ToString(),
                         dt.Rows[i]["WorkRemark"].ToString(),
                         Convert.ToDateTime(dt.Rows[i]["StartDate"].ToString()),
                         Convert.ToDateTime(dt.Rows[i]["EndDate"].ToString()),
                         Convert.ToDouble(dt.Rows[i]["Progress"].ToString())));
            }

            //task.ID = 1;
            //task.WorkName = "1";
            //task.ChargeName = "2";
            //task.WorkDetails = "3";
            //task.WorkRemark = "4";
            //task.StartDate = Convert.ToDateTime("2022-09-01");
            //task.EndDate = Convert.ToDateTime("2022-09-10");
            //task.Progress = Convert.ToDouble(5.1);

            //list.Add(task);

            //task.ID = 2;
            //task.WorkName = "11";
            //task.ChargeName = "22";
            //task.WorkDetails = "33";
            //task.WorkRemark = "44";
            //task.StartDate = Convert.ToDateTime("2022-09-01");
            //task.EndDate = Convert.ToDateTime("2022-09-20");
            //task.Progress = Convert.ToDouble(5.1);

            //list.Add(task);

            return list;
        }

        public static Task GetTask
            (int id,
             string workName,
             string chargeName,
             string workDetails,
             string workRemark,
             DateTime startDate,
             DateTime endDate,
             Double? Progress)
        {
            Task task = new Task();

            task.ID = id;
            task.WorkName = workName;
            task.ChargeName = chargeName;
            task.WorkDetails = workDetails;
            task.WorkRemark = workRemark;
            task.StartDate = startDate;
            task.EndDate = endDate;
            task.Progress2 = Progress;

            return task;
        }
    }
}
