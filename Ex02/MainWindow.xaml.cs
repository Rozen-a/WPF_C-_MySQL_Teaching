using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Ex02.Entity;
using MySql.Data.MySqlClient;

namespace Ex02
{
    public partial class MainWindow : Window
    {
        private DatabaseOperate dbOperate = new DatabaseOperate();

        public MainWindow()
        {
            InitializeComponent();  // 初始化组件
            this.Loaded += MainForm_Load;   // 初始化 ClassComboBox 
        }

        // 初始化 ClassComboBox 
        private void MainForm_Load(object sender, EventArgs e)

        {
            // 获取所有班级信息
            List<Class> classes = dbOperate.GetAllClass();

            // 清空 ComboBox 中的现有项，确保不重复添加
            ClassComboBox.Items.Clear();

            // 将班级信息添加到 ClassComboBox 
            foreach (Class class_ in classes)
            {
                ClassComboBox.Items.Add(class_.Classname);
            }

            // 设置默认选中项
            ClassComboBox.SelectedIndex = -1;

        }

        // “查询”按钮事件
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string studentId = StudentIdTextBox.Text;

            if (!string.IsNullOrEmpty(studentId))   // 输入框内容不为空
            {
                // 获取学生信息
                Student student = dbOperate.GetStudentsById(studentId);

                // 获取成功
                if (student != null)
                {
                    // 填充学生基本信息到对应的控件
                    StudentNameTextBox.Text = student.Studentname;
                    ClassComboBox.SelectedItem = dbOperate.GetClassById( student.Classid).Classname;
                    MaleRadioButton.IsChecked = student.Gender; // 如果是男生
                    FemaleRadioButton.IsChecked = !student.Gender; // 如果是女生

                    // 填充成绩表格
                    GradesDataGrid.ItemsSource = dbOperate.GetGrades(studentId);
                }
                else
                {
                    MessageBox.Show("未找到该学号的学生", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("学号不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // “|<”按钮事件（选中首行）
        private void FirstButton_Click(object sender, RoutedEventArgs e)
        {
            if (GradesDataGrid.SelectedIndex > 0)
                GradesDataGrid.SelectedIndex = 0;
        }

        // “<”按钮事件（选中上一行）
        private void PriorButton_Click(object sender, RoutedEventArgs e)
        {
            if (GradesDataGrid.SelectedIndex > 0)
                GradesDataGrid.SelectedIndex--;
        }

        // “>”按钮事件（选中下一行）
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (GradesDataGrid.SelectedIndex < GradesDataGrid.Items.Count - 1)
                GradesDataGrid.SelectedIndex++;
        }

        // “>|”按钮事件（选中尾行）
        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            GradesDataGrid.SelectedIndex = GradesDataGrid.Items.Count - 1;
        }

        // “新增”按钮事件
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string studentId = StudentIdTextBox.Text;
            string studentName = StudentNameTextBox.Text;
            bool gender = MaleRadioButton.IsChecked ?? false;
            string classId = dbOperate.GetClassByName(ClassComboBox.SelectedItem?.ToString()).Classid;

            if (string.IsNullOrEmpty(studentId))
            {
                MessageBox.Show("请填写学号", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (string.IsNullOrEmpty(studentName))
            {
                MessageBox.Show("请填写姓名", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (!(MaleRadioButton.IsChecked ?? false) && !(FemaleRadioButton.IsChecked ?? false))
            {
                MessageBox.Show("请选择性别", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (string.IsNullOrEmpty(classId))
            {
                MessageBox.Show("请选择班级", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                dbOperate.AddStudent(new Student
                {
                    Studentid = studentId,
                    Studentname = studentName,
                    Gender = gender,
                    Classid = classId,
                    Graduated = false
                });

                MessageBox.Show("学生信息已添加", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // “删除”按钮事件
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string studentId = StudentIdTextBox.Text;
            dbOperate.DeleteStudent(studentId);

            MessageBox.Show("学生及相关记录已删除", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // “保存”按钮事件
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取学生学号
            string studentId = StudentIdTextBox.Text;

            // 获取更新后的学生信息
            string studentName = StudentNameTextBox.Text;
            bool gender = MaleRadioButton.IsChecked ?? false;  // 假设“男”为true，“女”为false
            string classId = dbOperate.GetClassByName(ClassComboBox.SelectedItem?.ToString()).Classid;

            // 检查必填字段是否为空
            if (string.IsNullOrEmpty(studentId))
            {
                MessageBox.Show("请填写学号", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (string.IsNullOrEmpty(studentName))
            {
                MessageBox.Show("请填写姓名", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (!(MaleRadioButton.IsChecked ?? false) && !(FemaleRadioButton.IsChecked ?? false))
            {
                MessageBox.Show("请选择性别", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (string.IsNullOrEmpty(classId))
            {
                MessageBox.Show("请选择班级", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // 获取当前学生的记录
            var existingStudent = dbOperate.GetStudentsById(studentId);
            if (existingStudent != null && !string.IsNullOrEmpty(existingStudent.Studentid))
            {
                // 创建一个新的学生对象，使用更新后的信息
                var updatedStudent = new Student
                {
                    Studentid = studentId,
                    Studentname = studentName,
                    Gender = gender,
                    Classid = classId
                };

                // 调用更新方法，将更改保存到数据库
                dbOperate.UpdateStudent(updatedStudent, existingStudent.Studentid);

                // 提示用户学生信息已更新
                MessageBox.Show("学生信息已更新", "成功", MessageBoxButton.OK, MessageBoxImage.Information);

                // 刷新DataGrid以显示更新后的数据
                GradesDataGrid.ItemsSource = dbOperate.GetGrades(studentId);
            }
            else
            {
                MessageBox.Show("未找到该学生记录", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void male_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void female_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void classComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GradesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
