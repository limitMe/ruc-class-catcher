using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RUCClassCatcher.Model;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.IO;

namespace RUCClassCatcher
{
    class Program
    {
        static string access_token = "马赛克";
        static string begin_student = "52b6e99ea43a2c6313008437";

        static void Main(string[] args)
        {

            string tempKey = Console.ReadLine();
            if(tempKey != "")
            {
                access_token = tempKey;
            }

            //string userLoginPage = NetworkManager.getSinglton().sendGETRequestWithCallback("http://v.ruc.edu.cn/educenter/api/users/me?"
            //    +"school_domain =v.ruc.edu.cn&access_token=" + access_token, 5000, null);
            //UserHandlerModel handlerModel = JsonConvert.DeserializeObject<UserHandlerModel>(userLoginPage);
            //Console.WriteLine(handlerModel.data.users[0].id);

            refreshBeginStudent();
            FileStream fs = new FileStream("C:\\Users\\zhongdian\\Desktop\\c.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            try {
                while (true) {
                    Console.WriteLine("|--开始抓取学生" + begin_student + "课程");
                    sw.WriteLine("|--开始抓取学生" + begin_student + "课程");
                    string student_classes_api = "http://v.ruc.edu.cn/educenter/api/users/" + begin_student
                        + "/classes?offset=0&limit=30&type=%E6%99%AE%E9%80%9A%E8%AF%BE%E7%A8%8B&year=2015-2016&term="
                        + "%E7%A7%8B%E5%AD%A3%E5%AD%A6%E6%9C%9F&school_domain=v.ruc.edu.cn&&access_token=" + access_token;
                    var classesPageModel = JsonConvert
                        .DeserializeObject<StudentCourseModel>(NetworkManager
                        .getSinglton()
                        .sendGETRequestWithCallback(student_classes_api, 5000, null));
                    if(classesPageModel.code == 401)
                    {
                        refreshToken();
                        continue;
                    }
                    else if(classesPageModel.code != 200)
                    {
                        Console.WriteLine("发生未知错误：获取用户" + begin_student + "课表时返回了无法预料的值！");
                        sw.WriteLine("发生未知错误：获取用户" + begin_student + "课表时返回了无法预料的值！");
                    }
                    else
                    {
                        foreach (ClassInfoModel cls in classesPageModel.data.classes)
                        {
                            try
                            {
                                if (checkClassFinished(cls.id) == true)
                                {
                                    continue;
                                }
                            }
                            catch
                            {
                                Console.WriteLine("|----课程" + cls.name + "成员开始抓取!");
                                sw.WriteLine("|----课程" + cls.name + "成员开始抓取!");
                            }
                            string insertClassSql;
                            try
                            {
                                if (cls.schedule.schedule_weekly.Count > 1)
                                {
                                    insertClassSql = "insert into `class_table` (`classid`,`classname`,`teacherid`"
                                        + ",`startweek`,`endweek`,`placeone`,`placetwo`,`timeone`,`timetwo`,`isfinished`)"
                                        + "values('" + cls.id + "' ,'" + cls.name + "','" + cls.teachers_list[0]
                                        + "','" + cls.schedule.start_week + "','" + cls.schedule.end_week + "','"
                                        + cls.schedule.schedule_weekly[0].building + cls.schedule.schedule_weekly[0].room + "','"
                                        + cls.schedule.schedule_weekly[1].building + cls.schedule.schedule_weekly[1].room + "','"
                                        + cls.schedule.schedule_weekly[0].day + "-" + cls.schedule.schedule_weekly[0].start_section + "-" + cls.schedule.schedule_weekly[0].end_section + "','"
                                        + cls.schedule.schedule_weekly[1].day + "-" + cls.schedule.schedule_weekly[0].start_section + "-" + cls.schedule.schedule_weekly[1].end_section + "','"
                                        + "false');";
                                }
                                else
                                {
                                    insertClassSql = "insert into `class_table` (`classid`,`classname`,`teacherid`"
                                        + ",`startweek`,`endweek`,`placeone`,`timeone`,`isfinished`)"
                                        + "values('" + cls.id + "' ,'" + cls.name + "','" + cls.teachers_list[0]
                                        + "','" + cls.schedule.start_week + "','" + cls.schedule.end_week + "','"
                                        + cls.schedule.schedule_weekly[0].building + cls.schedule.schedule_weekly[0].room + "','"
                                        + cls.schedule.schedule_weekly[0].day + "-" + cls.schedule.schedule_weekly[0].start_section + "-" + cls.schedule.schedule_weekly[0].end_section + "','"
                                        + "false');";
                                }
                                DatabaseManager.getSinglton().excuteNonQuery(insertClassSql);
                            }
                            catch
                            {
                                Console.WriteLine("|----课程" + cls.name + "重复!");
                                sw.WriteLine("|----课程" + cls.name + "重复!");
                            }

                            string class_member_api = "http://v.ruc.edu.cn/educenter/api/classes/" + cls.id
                                + "/users?offset=0&limit=200&output_users=middle&school_domain=v.ruc.edu.cn&access_token=" + access_token;
                            var classMemberPageModel = JsonConvert
                                .DeserializeObject<ClassMemberModel>(NetworkManager
                                .getSinglton()
                                .sendGETRequestWithCallback(class_member_api, 5000, null));
                            if(classMemberPageModel.code == 401)
                            {
                                refreshToken();
                                continue;
                            }
                            else if(classMemberPageModel.code != 200)
                            {
                                Console.WriteLine("发生未知错误：获取课程" + cls.name + "成员时返回了无法预料的值！");
                                sw.WriteLine("发生未知错误：获取课程" + cls.name + "成员时返回了无法预料的值！");
                                continue;
                            }
                            else
                            {
                                foreach(UserInfoModel student in classMemberPageModel.data.users)
                                {
                                    try
                                    {
                                        Random rand = new Random();
                                        int random = rand.Next(100000);
                                        string insertStudentSql = "insert into `student_table` "
                                            + "(`ssid`,`studentid`,`studentname`,`school`,`random`)"
                                            + "values('" + student.identifier[0].idno + "','" + student.id + "','" + student.name + "','"
                                            + student.identifier[0].dep_name + "',"+random+");";
                                        DatabaseManager.getSinglton().excuteNonQuery(insertStudentSql);
                                        Console.WriteLine("|------学生" + student.name + "已被由课程" + cls.name + "写入!");
                                        sw.WriteLine("|------学生" + student.name + "已被由课程" + cls.name + "写入!");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("|------课程" + cls.name + "检测到重复学生"+student.name+"!");
                                        sw.WriteLine("|------课程" + cls.name + "检测到重复学生" + student.name + "!");
                                    }
                                }
                            }
                            Console.WriteLine("|----课程" + cls.name + "成员已经抓取完毕!");
                            sw.WriteLine("|----课程" + cls.name + "成员已经抓取完毕!");
                            string updateClassSql = "update `class_table` SET `isfinished`= 1 where classid = '" + cls.id +"'";
                            DatabaseManager.getSinglton().excuteNonQuery(updateClassSql);
                        }
                        Console.WriteLine("|--学生" + begin_student + "课程已经抓取完毕!");
                        sw.WriteLine("|--学生" + begin_student + "课程已经抓取完毕!");
                        string updateStudentSql = "update `student_table` SET `isfinished` = 1 where studentid = '" + begin_student + "'";
                        DatabaseManager.getSinglton().excuteNonQuery(updateStudentSql);
                        refreshBeginStudent();
                    }
                }
             }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                sw.WriteLine(ex.Message);
            }
            sw.Close();
            fs.Close();
            Console.ReadKey();
        }

        static void refreshToken()
        {
        }

        static void refreshBeginStudent()
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseManager.connStr))
            {
                lock (conn)
                {
                    conn.Open();
                    string sql = "select `studentid` from `student_table` where isfinished is null ORDER BY random DESC LIMIT 1;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    begin_student = reader[0].ToString();
                }
            }
        }

        static bool checkClassFinished(string classid)
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseManager.connStr))
            {
                lock (conn)
                {
                    conn.Open();
                    string sql = "select `isfinished` from `class_table` where classid = '" + classid + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    //Console.WriteLine(reader[0].ToString());
                    return Boolean.Parse(reader[0].ToString());
                }
            }
        }
    }
}
