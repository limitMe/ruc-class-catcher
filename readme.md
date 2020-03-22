
# RUC追课

RUC追课是2015年上架AppStore帮助人民大学同学找寻课堂小伙伴的 iOS App: https://github.com/limitMe/ruc-class-follower-ios

### 功能

在RUC追课上，你可以通过姓名或者学号，将感兴趣的同学加入关注。然后你关注的同学的课程活动将会按时间顺序展示在你的时间线上。你还可以访问同学的RUC追课主页给TA留言。

### 架构

ruc-class-follower-iOS 是采用Objective-C编写的RUC追课的iOS客户端，也是唯一的客户端。
ruc-class-follower-service 是采用C#编写的Aspx .Net 4.5服务端。

**ruc-class-catcher**（本repo） 是采用C#编写的.Net 4.5控制台应用。原理：人大校内网暴露了通过学生uuid查询当前学生课程，通过课程id查询该班级学生，通过从我自己的uuid开始，不花几层就能爬取完全校学生当前选课信息。
