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
BASE_DB_DATABASE = 'contra_base_pingshen'
BASE_DB_CHARSET = 'utf8'

LANGUAGE_TABLE = 'language_1'
SKIPED_FILE_TAG = '[local no use]'
SKIPED_FILE_NAME = ['language', 'sensitive_word']

SPECIAL_HANDLE_FILE = 'game_lang'

cursor = None
db_conn = None

# 设置数据库
# engine = create_engine('mysql+pymysql://chengxu:changit2017@192.168.199.156:3306/contra_base_en_test_cxf?charset=utf8', echo=False)
engine = create_engine('''mysql+pymysql://%s:%s@%s:%s/%s?charset=%s'''
    %(BASE_DB_USER, BASE_DB_PASSWORD, BASE_DB_HOST, BASE_DB_PORT, BASE_DB_DATABASE, BASE_DB_CHARSET), echo=False)
if engine is None:
    raise Exception("conn err")
DBSession = sessionmaker(bind=engine)
base_map = automap_base()
base_map.prepare(engine, reflect=True)

def getColNameList(tableName):
    # 获取列所有信息
    sql = """ SELECT column_name, COLUMN_COMMENT FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = '%s' and TABLE_SCHEMA='%s'""" %(tableName, BASE_DB_DATABASE)
    cursor.execute(sql)
    colInfo = cursor.fetchall()
    colNameList = []
    for row in colInfo:
        if "[Language]" in row[1]:
            colNameList.append(row[0])
    return colNameList

# 写入language表
def updataLanguageTable(key, value):
    #判断这张表是否存在，若存在，则跳过创建表操作
    sql1 = """CREATE TABLE IF NOT EXISTS `%s` (`id` varchar(50) NOT NULL,  `lang` text default NULL,   PRIMARY KEY (`id`)) """ %(LANGUAGE_TABLE)
    cursor.execute(sql1)
    value = value.replace("'", "\\'")
    sql2 = """ REPLACE INTO `%s` (`%s`, `%s`) VALUES ('%s', '%s') """ %(LANGUAGE_TABLE, "id", "lang", key, value)   
    print sql2
    result = cursor.execute(sql2)
    if result < 1:
        print 'failed to replace key %s value %s of table %s' %(key, value, LANGUAGE_TABLE)

def handleSpecialFile(tableInfo):
    tableName = tableInfo['name']
    session = DBSession()
    table_class = base_map.classes[tableName]
    if table_class is None:
        raise Exception("table name err")
    records = session.query(table_class).all()
    if len(records) <= 0:
        print 'table is empty! name=' + tableName
        return
    for id in range(len(records)):
        record = records[id]
        updataLanguageTable(record.id, record.lang_1)
    session.commit()
    session.close()


def writeToLanguage(tableInfo):
    tableName = tableInfo['name']
    tableComment = tableInfo['comment'] 
    if tableName in SKIPED_FILE_NAME or  SKIPED_FILE_TAG in tableComment:
        print 'skip table name ' + tableName
        return

    if tableName == SPECIAL_HANDLE_FILE:
        print '='*10 + SPECIAL_HANDLE_FILE
        handleSpecialFile(tableInfo)
        return    
    session = DBSession()
    table_class = base_map.classes[tableName]
    if table_class is None:
        raise Exception("table name err")
    records = session.query(table_class).all()
    if len(records) <= 0:
        print 'table is empty! name=' + tableName
        return
    colNameList = getColNameList(tableName)
    for colName in colNameList:
        for id in range(len(records)):
            record = records[id]
            key = tableName + "_" + colName + "_" + bytes(id+1)
            val = getattr(record, colName)
            if( tableName + "_" + colName not in val) :
                # 写入language表
                updataLanguageTable(key, val)
                # 把key写入表 
                setattr(record, colName, key)
    session.commit()
    session.close()

# 获取表名和注释
def getTableInfoList():
    global cursor
    global db_conn
    db_conn = pymysql.connect(host=BASE_DB_HOST,
                                    port=BASE_DB_PORT,
                                    charset=BASE_DB_CHARSET,
                                    user=BASE_DB_USER,
                                    passwd=BASE_DB_PASSWORD,
                                    db=BASE_DB_DATABASE,
                                    cursorclass=Cursor)
    cursor = db_conn.cursor()
    sql = """SELECT DISTINCT TABLE_NAME, TABLE_COMMENT FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='%s';""" %BASE_DB_DATABASE
    cursor.execute(sql)
    result = cursor.fetchall()
    tableInfoList = []
    for row in result:
        val = {'name':row[0], 'comment':row[1]}
        tableInfoList.append(val)
    return tableInfoList


    
def main():
    tableInfoList = getTableInfoList()
    for tableInfo in tableInfoList:
        writeToLanguage(tableInfo)

if __name__ == '__main__':
    main()

    # print sys.argv
    # if len(sys.argv) < 2:
    #     print "error arguments, need language tag!"
    #     sys.exit()
    # if sys.argv[1] == '0':
    #     BASE_DB_DATABASE = 'contra_base_test'
    # elif sys.argv[1] == '1':
    #     BASE_DB_DATABASE = 'contra_base_en_test'


