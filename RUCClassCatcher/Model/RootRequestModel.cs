using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUCClassCatcher.Model
{
    public class RootRequestModel
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    public class CodedUserModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string photo { get; set; }
        public string description { get; set; }
    }

    public class DataWithUserList
    {
        public List<CodedUserModel> users { get; set; }
    }

    public class WeekSchedule
    {
        public int day { get; set; }
        public int start_section { get; set; }
        public int end_section { get; set; }
        public string room { get; set; }
        public string building { get; set; }
    }

    public class Schedule
    {
        public string year { get; set; }
        public int start_week { get; set; }
        public int end_week { get; set; }
        public List<WeekSchedule> schedule_weekly { get; set; }
        public string term { get; set; }
    }

    public class ClassInfoModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<string> teachers_list { get; set; }
        public Schedule schedule { get; set; }
    }

    public class DataWithClassAndUserList
    {
        public List<CodedUserModel> users { get; set; }
        public List<ClassInfoModel> classes { get; set; }
    }

    public class Identifier
    {
        public string dep_name { get; set; }
        public string idno { get; set; }
    }

    public class UserInfoModel:CodedUserModel
    {
        public List<Identifier> identifier { get; set; }
    }

    public class DataWithUserInfoList
    {
        public List<UserInfoModel> users { get; set; }
    }
}
