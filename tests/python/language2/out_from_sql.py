#!/usr/bin/python
#-* coding:UTF-8 -*

import pymysql
from pymysql.cursors import DictCursor, Cursor
import json
import sys
import csv
import os
import codecs

import sys
reload(sys)
sys.setdefaultencoding('utf-8')

# SKIPED_FILE = ['brawl_data', 'map_born']

abspath = os.path.abspath(sys.argv[0])
dname = os.path.dirname(abspath)
os.chdir(dname)


BASE_DB_HOST = '192.168.199.156'
BASE_DB_USER = 'cehua'
BASE_DB_PORT = 3306
BASE_DB_PASSWORD = 'changitfjfb2017'
BASE_DB_DATABASE = 'contra_base_test'
BASE_DB_CHARSET = 'utf8'


# 创建文件夹
def createFile():
    path = dname + "\\translateData\\"
    if not os.path.exists(path):
        os.mkdir(path)
    os.system("del /S/Q %s" % path)
    return path

# 检查表名和列名
def checkTableAndCol(table, colname):
    print("cloname len =", len(colname))
    if len(colname) <= 0:
        return False
    print("table=>", table)
    print("colname=>", colname)
    return True
# 获取表数据
def getTabelData(table, colname):
    db_conn = pymysql.connect(host=BASE_DB_HOST,
                            port=BASE_DB_PORT,
                            charset=BASE_DB_CHARSET,
                            user=BASE_DB_USER,
                            passwd=BASE_DB_PASSWORD,
                            db=BASE_DB_DATABASE,
                            cursorclass=Cursor)
    cursor = db_conn.cursor()
    # 拼接sql语句
    sql = """select """
    for index in range(len(colname)):
        if index < len(colname)-1:
            sql = sql + colname[index] + ","
        else:
            sql = sql + colname[index]
    sql = sql + " from " + table

    try:
        cursor.execute(sql)
    #异常走这里
    except:
        print("====================>>>")
        print("请检查错误以下SQL语句是否正确：".decode("utf-8") )
        print sql
        print("====================<<<")
    #没有异常走这里
    else:
        data = cursor.fetchall()
        if len(data) > 0:
            return data
  

# 写入文件
def writeToFile(path, tablename, colname, data):
    # print("get colname==>>", colname)
    # file_object = open(path + "%s.txt"%tablename, 'wb')
    file_object = codecs.open(path + "%s.txt"%tablename, 'w', 'utf-8')
    h = csv.writer(file_object,  quoting=csv.QUOTE_NONE)
    h.writerow(colname)
    # file_object.close()
    # file_object = open(path + "item.txt", 'ab')
    w = csv.writer(file_object, delimiter = ',', quotechar = '"', quoting = csv.QUOTE_ALL)
    for d in data:
        w.writerow(d)
    file_object.close()

def translateData():
    # 创建文件
    path =createFile()
    file_object = open(dname + "\\translate.txt", 'r')
    lines = file_object.readlines()#读取全部内容
    
    for index in range(len(lines)):
        if index != 0:
            line = lines[index]
            info = line.split()
            # print("info==>>", info)
            # 真实的表，用于sql查询
            colname = []
            # 写入文件的表明，用于替换时的查询
            rcolname = []
            for index in range(len(info)):
                # print("index===>", index, len(info[index]))
                if index == 0:
                    tablename = info[index]
                elif index == 1:
                    val = info[index]
                    # print("index2==>", val)
                    match = val.strip(' " ')
                    match = match.split(',')
                    # print("match==>", match)
                    for n in match:
                        colname.append(n)
                        rcolname.append('$'+n)
                else:
                    colname.append(info[index])
                    rcolname.append(info[index])
                # print("colname=>", colname)
                # print("rcolname==>", rcolname)

            if checkTableAndCol(tablename, colname):
                # 获取表数据
                data = getTabelData(tablename, colname)
                # 写入文件
                writeToFile(path, tablename, rcolname, data)

    file_object.close()

translateData()
