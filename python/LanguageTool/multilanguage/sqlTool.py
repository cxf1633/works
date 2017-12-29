#!/usr/bin/python
#-* coding:UTF-8 -*

import pymysql
from pymysql.cursors import DictCursor, Cursor
import json
import csv
import os
import codecs
import sys
reload(sys)
sys.setdefaultencoding('utf-8')

abspath = os.path.abspath(sys.argv[0])
dname = os.path.dirname(abspath)
os.chdir(dname)

# 导入:
from sqlalchemy import Column, Integer, String, create_engine
from sqlalchemy.orm import sessionmaker
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.ext.automap import automap_base 
from sqlalchemy.orm import aliased

# contra_base_test 是原始库 不可修改
# contra_base_en_test 是英文库，可以修改

BASE_DB_HOST = '192.168.199.156'
BASE_DB_USER = 'chengxu'
BASE_DB_PORT = 3306
BASE_DB_PASSWORD = 'changit2017'
BASE_DB_DATABASE = 'contra_base_test_cxf'
BASE_DB_CHARSET = 'utf8'


def main():
    db_conn = pymysql.connect(host=BASE_DB_HOST,
                                    port=BASE_DB_PORT,
                                    charset=BASE_DB_CHARSET,
                                    user=BASE_DB_USER,
                                    passwd=BASE_DB_PASSWORD,
                                    db=BASE_DB_DATABASE,
                                    cursorclass=Cursor)
    cursor = db_conn.cursor()

    table_name =  'skill_effect'
    sql = """ SELECT COLUMN_NAME, COLUMN_COMMENT,COLUMN_KEY FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = %s  AND TABLE_SCHEMA = %s """
    cursor.execute(sql, [table_name, BASE_DB_DATABASE])
    result = cursor.fetchall()
    for row in result:
        if row[2] == "PRI":
            print "aa"
    
    k = -1
    o = True
    if k >0:
        o = True
    else:
        o = False        
    print o
    print "ss"

if __name__ == '__main__':
    main()