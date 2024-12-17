using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Ex02.Entity;
using System.Collections;
using System.Transactions;
using System.Windows;

namespace Ex02
{
    public class DatabaseOperate
    {
        private string connectionString = "server=localhost;database=teaching;user=rozen;password=246879";

        // 获取所有班级信息
        public List<Class> GetAllClass()
        {
            var classes = new List<Class>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM classes";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        classes.Add(new Class
                        {
                            Classid = reader["Classid"].ToString(),
                            Classname = reader["Classname"].ToString()
                        });
                    }

                }
            }

            return classes;
        }

        // 通过 id 获取班级信息
        public Class GetClassById(string Classid)
        {
            Class class_ = new Class();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM classes WHERE Classid = @Classid";
                MySqlCommand cmd = new MySqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@Classid", Classid);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        class_.Classid = reader["Classid"].ToString();
                        class_.Classname = reader["Classname"].ToString();
                    }
                }
            }

            return class_;
        }

        // 通过 name 获取班级信息
        public Class GetClassByName(string Classname)
        {
            Class class_ = new Class();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM classes WHERE Classname = @Classname";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Classname", Classname);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        class_.Classid = reader["Classid"].ToString();
                        class_.Classname = reader["Classname"].ToString();
                    }
                }
            }

            return class_;
        }

        // 获取学生信息
        public Student GetStudentsById(string studentId)
        {
            Student student = new Student();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Students WHERE Studentid = @Studentid";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Studentid", studentId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // 使用set方法赋值
                        student.Studentid = reader["Studentid"].ToString();
                        student.Studentname = reader["Studentname"].ToString();
                        student.Gender = Convert.ToBoolean(reader["Gender"]);
                        student.Classid = reader["Classid"].ToString();
                        student.Graduated = Convert.ToBoolean(reader["Graduated"]);
                    }
                }
            }
            return student;
        }

        // 获取学生成绩
        public List<Elective> GetGrades(string studentId)
        {
            var grades = new List<Elective>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Elective WHERE Studentid = @Studentid";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Studentid", studentId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        grades.Add(new Elective
                        {
                            Studentid = reader["Studentid"].ToString(),
                            Subjectid = reader["Subjectid"].ToString(),
                            Grade = Convert.ToInt32(reader["Grade"])
                        });
                    }
                }
            }
            return grades;
        }

        // 新增学生信息
        public void AddStudent(Student student)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();

                try
                {
                    string query = "INSERT INTO Students (Studentid, Studentname, Gender, Classid, Graduated) VALUES (@Studentid, @Studentname, @Gender, @Classid, @Graduated)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Studentid", student.Studentid);
                    cmd.Parameters.AddWithValue("@Studentname", student.Studentname);
                    cmd.Parameters.AddWithValue("@Gender", student.Gender);
                    cmd.Parameters.AddWithValue("@Classid", student.Classid);
                    cmd.Parameters.AddWithValue("@Graduated", student.Graduated);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();  // 提交事务
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"数据库操作失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw;
                }

            }
        }

        // 删除学生信息及选课记录
        public void DeleteStudent(string studentId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();

                try
                {
                    string deleteElective = "DELETE FROM Elective WHERE Studentid = @Studentid";
                    MySqlCommand cmd = new MySqlCommand(deleteElective, conn, transaction);
                    cmd.Parameters.AddWithValue("@Studentid", studentId);
                    cmd.ExecuteNonQuery();

                    string deleteStudent = "DELETE FROM Students WHERE Studentid = @Studentid";
                    cmd = new MySqlCommand(deleteStudent, conn, transaction);
                    cmd.Parameters.AddWithValue("@Studentid", studentId);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // 更新学生信息
        public void UpdateStudent(Student student, string originalStudentId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();

                try
                {
                    string query = "UPDATE Students SET Studentid = @Studentid, Studentname = @Studentname, Gender = @Gender, Classid = @Classid, Graduated = @Graduated WHERE Studentid = @OriginalStudentid";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Studentid", student.Studentid);
                    cmd.Parameters.AddWithValue("@Studentname", student.Studentname);
                    cmd.Parameters.AddWithValue("@Gender", student.Gender);
                    cmd.Parameters.AddWithValue("@Classid", student.Classid);
                    cmd.Parameters.AddWithValue("@Graduated", student.Graduated);
                    cmd.Parameters.AddWithValue("@OriginalStudentid", originalStudentId);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
