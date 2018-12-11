#!/usr/bin/python
#-* coding:UTF-8 -*

import pymysql
from pymysql.cursors import DictCursor, Cursor

import subprocess
from subprocess import Popen, PIPE, STDOUT, call

import json
import csv
import os
import codecs
import sys
reload(sys)
# os.system("ls")
sys.setdefaultencoding('utf-8')
abspath = os.path.abspath(sys.argv[0])
dname = os.path.dirname(abspath)
os.chdir(dname)
# 翻译过后的文件
dataPath = dname + "\\translateData\\"

# contra_base_test 是原始库 不可修改
# contra_base_en_test 是英文库，可以修改

# 初始化数据库连接:
# engine = create_engine('mysql+pymysql://chengxu:changit2017@192.168.199.156:3306/contra_base_en_test')
# 清空数据库
# r = connection.execute("delete from " + tableName)

BASE_DB_HOST = '192.168.199.156'
BASE_DB_USER = 'chengxu'
BASE_DB_PORT = 3306
BASE_DB_PASSWORD = 'changit2017'
BASE_DB_DATABASE = 'contra_base_en_test'
BASE_DB_CHARSET = 'utf8'

#连接数据库
db_conn = pymysql.connect(host=BASE_DB_HOST,
                        port=BASE_DB_PORT,
                        charset=BASE_DB_CHARSET,
                        user=BASE_DB_USER,
                        passwd=BASE_DB_PASSWORD,
                        db=BASE_DB_DATABASE,
                        cursorclass=Cursor)
#创建游标
cursor = db_conn.cursor()

def updataSql(fileName):
    tableName = fileName.replace('.txt', '').strip()
    # 判定数据库是否有这个表
    stmt = "SHOW TABLES LIKE '%s'" %(tableName)
    # print stmt
    cursor.execute(stmt)
    result = cursor.fetchone()
    if not result:
        print "no table:" + tableName
        return 

    if not os.path.exists(dataPath + fileName):
        print("=======================skip file name %s !========================" % fileName)
        return
    destFile = codecs.open(dataPath + fileName, 'r', 'utf-8-sig')
    tr = csv.reader(destFile, delimiter=',',quotechar='', quoting=csv.QUOTE_NONE)
    
    # 列名
    titles = tr.next()
    colString = []
    for t in titles:
        # print "t", t
        colString.append(t)
    # 内容
    dr = csv.reader(destFile, delimiter=',',quotechar='"', quoting=csv.QUOTE_ALL)
    dataString = []
    for row in dr:
        data = []
        for s in row:
            data.append(s)
        dataString.append(data)
    # print dataString[15]
    # return
    
    # ====================数据库部分===================
    # 查询表的列名
    sql = """ SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = '%s' and TABLE_NAME = '%s'""" % (BASE_DB_DATABASE, tableName)
    cursor.execute(sql)
    coltup = cursor.fetchall()
    # sql关键字
    sqlKeyList = ["condition"]
    # 列名存在并且合法
    def checkColName(name):
        rt = False
        for c in coltup:
            if c[0] == name:
                rt = True
                break
        if rt:
            if name in sqlKeyList:
                return '`'+name+'`'
            else:
                return name
        else:
            return
    # checkColName("condition")
    # return
    # 内容中有""" 获取‘’添加转义字符
    def transferContent(content):
        if content is None:
            return None
        else:
            string = ""
            for c in content:
                if c == '"':
                    string += '\\\"'
                elif c == "'":
                    string += "\\\'"
                elif c == "\\":
                    string += "\\\\"
                else:
                    string += c
            return string

    def updataOne(dataString):
        sqlString = "UPDATE " + tableName + " SET "
        # sqlString = "UPDATE ability_score SET type=%s WHERE id=%s " % ("22", "1")
        setString = ""
        # print len(setString)
        whereString = ""
        for id in range(len(colString)):
            data = transferContent(dataString[id])
            if not data:
                continue
            col = ""
            if colString[id].find("$") != -1:
                col = colString[id].replace('$', '').strip()
                colname = checkColName(col)
                if colname:
                    if len(whereString)>0:
                        whereString = whereString + " AND " + colname + "=" + '"' + data + '"'
                    else:
                        whereString = " WHERE " + colname + "=" + '"' + data + '"'
                else:
                    print "table:%s no exist colname:%s" %(tableName, col)
            else:
                col = colString[id]
                colname = checkColName(col)
                if colname:
                    if len(setString) > 0:
                        setString = setString + ","+ colname + "=" + '"' + data + '"'
                    else:
                        setString = colname + "=" + '"' + data + '"'
                else:
                    print "table:%s no exist colname:%s" %(tableName, col)
    
        if len(setString) <= 0:
            return
        sqlString = sqlString + setString + whereString
        # print sqlString
        
        try:
            cursor.execute(sqlString)
        #异常走这里
        except:
            print("====================>>>")
            print("请检查错误以下SQL语句是否正确：".decode("utf-8") )
            print sqlString
            print("====================<<<")
            raw_input("Press Enter to exit...")
            sys.exit(1)
        #没有异常走这里
        # else:
            # print "success"

    # updataOne(dataString[15])
    # 翻译所有的行
    for data in dataString:
        updataOne(data)

    db_conn.commit()






# 测试游标形式
def testCursor():
    # cursor = db_conn.cursor(cursor=pymysql.cursors.Cursor)
    cursor = db_conn.cursor(cursor=pymysql.cursors.DictCursor)
    sql = "SELECT * FROM contra_base_en_cxf.ability_score WHERE id=1"
    cursor.execute(sql)
    ret = cursor.fetchall()
    # 列名 condition_name 
    print(ret[0]["condition_name"])

# 创建数据库
def createDatabase(database):
    if not database:
        print "数据库名不正确"
        return
    # CREATE table IF NOT EXISTS
    sql_create = '''CREATE DATABASE `%s` ''' %(database)
    print "sql_create===>", sql_create
    cursor.execute(sql_create)
# 删除数据库
def dropDatabase(database):
    if not database:
        print "数据库名不正确"
        return
    sql_drop = ''' DROP DATABASE IF EXISTS `%s` ''' %(database)
    print "sql_drop===>", sql_drop
    cursor.execute(sql_drop)
# 主程序
def main():    
    # 创建数据库 
    database = "contra_base_en_cxf"
    dropDatabase(database)
    createDatabase(database)
    # # # 复制数据库
    sql_souceDatabase = '''mysqldump -h 192.168.199.156 -P 3306 -uchengxu -pchangit2017 %s''' %("contra_base_test")
    sql_destDatabase = '''mysql -h 192.168.199.156 -P 3306 -uchengxu -pchangit2017 %s''' %(database) 
    sql_copyDatabase = sql_souceDatabase + ''' | sed -e "s/\\\'/''/g" | '''+  sql_destDatabase
    print sql_copyDatabase
    subprocess.Popen(sql_copyDatabase, shell=True)
    
    # ss = '''mysql -h 192.168.199.156 -P 3306 -uchengxu -pchangit2017 %s <123.sql ''' %("contra_base_en_cxf") 
    # print ss
    # subprocess.Popen(ss, shell=True)
  
    #替换翻译字段 
    # files = os.listdir(dataPath)
    # for v in files:
    #     updataSql(v)
    # cursor.close()
    # db_conn.close()

    # 备份 contra_base_test 库
    # ss = '''mysqldump -h 192.168.199.156 -P 3306 -uchengxu -pchangit2017 %s | sed -e "s/\\\'/''/g" >123.sql ''' %("contra_base_test") 
    # print ss
    # subprocess.Popen(ss, shell=True)
# ========>>主程序
main()