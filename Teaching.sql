-- 切换数据库
USE Teaching;

-- 删除旧表
DROP TABLE IF EXISTS Elective;
DROP TABLE IF EXISTS Students;
DROP TABLE IF EXISTS Classes;
DROP TABLE IF EXISTS Subjects;

-- 创建表
-- 班级表
CREATE TABLE Classes (
    Classid CHAR(6) NOT NULL,
    Classname CHAR(30) NOT NULL,
    PRIMARY KEY (Classid)
);

-- 学生表
CREATE TABLE Students (
    Studentid CHAR(11) NOT NULL,
    Classid CHAR(6) NOT NULL,
    Studentname CHAR(30) NOT NULL,
    Gender BIT NOT NULL,
    Graduated BIT NOT NULL,
    PRIMARY KEY (Studentid),
    FOREIGN KEY (Classid) REFERENCES Classes(Classid)
);

-- 课程表
CREATE TABLE Subjects (
    Subjectid CHAR(6) NOT NULL,
    Subjectname CHAR(30) NOT NULL,
    PRIMARY KEY (Subjectid)
);

-- 选课表
CREATE TABLE Elective (
    Studentid CHAR(11) NOT NULL,
    Subjectid CHAR(6) NOT NULL,
    Grade INT NOT NULL,
    PRIMARY KEY (Studentid, Subjectid),
    FOREIGN KEY (Studentid) REFERENCES Students(Studentid),
    FOREIGN KEY (Subjectid) REFERENCES Subjects(Subjectid)
);

-- 插入数据
INSERT INTO Classes VALUES
('SA02', '软工A2'),
('SA03', '软工A1'),
('SA01', '软工A1');

INSERT INTO Students VALUES
('20150101002', 'SA02', '张晓燕', 0, 0),
('20150101001', 'SA01', '王小三', 1, 0),
('20150101004', 'SA01', '李四', 1, 0);

INSERT INTO Subjects VALUES
('Sub001', '大学英语'),
('Sub002', '程序设计基础'),
('Sub003', '数据库系统概论'),
('Sub004', '高等数学'),
('Sub005', '数据结构');

INSERT INTO Elective VALUES
('20150101001', 'Sub001', 88),
('20150101001', 'Sub002', 93),
('20150101002', 'Sub001', 78),
('20150101002', 'Sub002', 75),
('20150101002', 'Sub003', 85);

-- 打印所有数据
SELECT * FROM Students;
SELECT * FROM Classes;
SELECT * FROM Subjects;
SELECT * FROM Elective;
