#!/usr/bin/env python
#-* coding:UTF-8 -*

import pymysql
from pymysql.cursors import DictCursor, Cursor
import json
import sys
import csv
import os

import sys
reload(sys)
sys.setdefaultencoding('utf-8')
import os.path
 

SKIPED_FILE_TAG = '[local no use]'

BASE_DB_HOST = '192.168.199.156'
BASE_DB_USER = 'chengxu'
BASE_DB_PORT = 3306
BASE_DB_PASSWORD = 'changit2017'
BASE_DB_DATABASE = 'contra_base_test'
BASE_DB_CHARSET = 'utf8'

abspath = os.path.abspath(sys.argv[0])
dname = os.path.dirname(abspath)
os.chdir(dname)
path =  dname + "\\..\\oldFiles\\"

def getBaseData():
    db_conn = pymysql.connect(host=BASE_DB_HOST,
                                    port=BASE_DB_PORT,
                                    charset=BASE_DB_CHARSET,
                                    user=BASE_DB_USER,
                                    passwd=BASE_DB_PASSWORD,
                                    db='information_schema',
                                    cursorclass=DictCursor)
    cursor = db_conn.cursor()
    sql = """ SELECT TABLE_NAME,TABLE_COMMENT FROM TABLES WHERE table_schema = %s """
    cursor.execute(sql, BASE_DB_DATABASE)
    result = cursor.fetchall()

    tableField = dict()
    for tableName in result:
        sql = """ SELECT COLUMN_NAME FROM COLUMNS WHERE TABLE_SCHEMA = '%s' and TABLE_NAME = '%s'""" % (BASE_DB_DATABASE, tableName["TABLE_NAME"])
        cursor.execute(sql)
        ret = cursor.fetchall()
        tableField[tableName["TABLE_NAME"]] = [x['COLUMN_NAME'] for x in ret]

    tables = {}
    db_conn = pymysql.connect(host=BASE_DB_HOST,
                                port=BASE_DB_PORT,
                                charset=BASE_DB_CHARSET,
                                user=BASE_DB_USER,
                                passwd=BASE_DB_PASSWORD,
                                db=BASE_DB_DATABASE,
                              cursorclass=Cursor)

    if not os.path.exists(path):
        os.mkdir(path)
    os.system("del /S/Q %s" % path)
	
    for row in result:
        tableName = row["TABLE_NAME"]
        tableComment = row['TABLE_COMMENT']

        if SKIPED_FILE_TAG in tableComment:
            print '!!skip table name ' + tableName
            continue
        print "tableName=",tableName
        cursor = db_conn.cursor()
        sql = """select * from `%s`""" % tableName
        cursor.execute(sql)
        data = cursor.fetchall()
        if len(data) > 0:
            file_object = open(path.decode("GBK") + "%s.txt" %tableName, 'wb')
            h = csv.writer(file_object,  quoting=csv.QUOTE_NONE)
            h.writerow(tableField[tableName])
            file_object.close()
            file_object = open(path.decode("GBK") + "%s.txt" %tableName, 'ab')
            w = csv.writer(file_object, delimiter = ',', quotechar = '"', quoting = csv.QUOTE_ALL)
            for d in data:
                w.writerow(d)
            file_object.close()

if __name__ == '__main__':
    print sys.argv
    if len(sys.argv) < 2:
        print "error arguments, need language tag!"
        sys.exit()
    if sys.argv[1] == '0':
        BASE_DB_DATABASE = 'contra_base_test'
    elif sys.argv[1] == '1':
        BASE_DB_DATABASE = 'contra_base_en_test'

        
    # abspath = os.path.abspath(sys.argv[0])
    # dname = os.path.dirname(abspath)
    # os.chdir(dname)
    # if sys.argv[1] == '0':
    #     path = dname + "\\..\\..\\RefResources\\DatabaseCN\\"
    #     DataBase_Name = setting.BASE_DB_DATABASE_CN
    # elif sys.argv[1] == '1':
    #     path = dname + "\\..\\..\\RefResources\\DatabaseEN\\"
    #     DataBase_Name = setting.BASE_DB_DATABASE_EN
    getBaseData()
