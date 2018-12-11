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
BASE_DB_DATABASE = 'contra_base_en_test_cxf'
BASE_DB_CHARSET = 'utf8'

TEST_TABLE = 'ability_score'

LANGUAGE_TABLE = 'language_en'
SKIPED_FILE_TAG = '[local no use]'
SKIPED_LANGUAGE = 'language'

# def getBaseData():

#     db_conn = pymysql.connect(host=BASE_DB_HOST,
#                         port=BASE_DB_PORT,
#                         charset=BASE_DB_CHARSET,
#                         user=BASE_DB_USER,
#                         passwd=BASE_DB_PASSWORD,
#                         db=BASE_DB_DATABASE)
#     #默认游标
#     # cursor = db_conn.cursor(cursor=pymysql.cursors.Cursor)
#     # cursor = db_conn.cursor()
#     #设置游标为字典类型
#     cursor = db_conn.cursor(cursor=pymysql.cursors.DictCursor)

#     # 获取列所有信息
#     sql = """ show full columns from %s """ %(TEST_TABLE)
#     cursor.execute(sql)
#     colInfo = cursor.fetchall()
#     colNameList = []
#     for row in colInfo:
#         # print("Comment=", row['Comment'])
#         if "[Language]" in row['Comment']:
#             print("colName=", row['Field'])
#             colNameList.append(row['Field'])

    
#     #获取列数据 
#     colName = "ability_name"
#     sql = """ SELECT %s FROM %s """ %(colName, TEST_TABLE)
#     cursor.execute(sql)
#     colData = cursor.fetchall()
#     print("sdfsdf")
#     col_1 = "id"
#     col_2 = "lang"
#     for id in range(len(colData)):
#         key = TEST_TABLE + "_" + colName + "_" + bytes(id+1)
#         sql = """ REPLACE INTO `%s` (`%s`) VALUES ('%s') """ %(TEST_TABLE, colName, key)
#         cursor.execute(sql)
#         # 把内容写入language表
#         sql2 = """ REPLACE INTO `%s` (`%s`, `%s`) VALUES ('%s', '%s') """ %(LANGUAGE_TABLE, col_1, col_2, key, colData[id][colName])
#         cursor.execute(sql2)

#     db_conn.commit()

   ###############################################
    # sql = ''' INSERT INTO `language_en` (`id`, `lang`) VALUES ('ability_score_ability_name_3', 'Killing') '''
    # sql = """UPDATE `language_en` SET `lang`='Killing3' WHERE `id`='ability_score_ability_name_1'; """
    # sql = """ REPLACE INTO `language_en` (`id`, `lang`) VALUES ('ability_score_ability_name_3', 'Killing3') """
    # sql = """ REPLACE INTO `language_en` (`%s`, `%s`) VALUES ('%s', '%s') """ %("id", "lang", "ability_score_ability_name_6", "Killing6")
    # col_1 = "id"
    # col_2 = "lang"
    # key = "ability_score_ability_name_7"
    # value = "Killing7"
    # sql = """ REPLACE INTO `language_en` (`%s`, `%s`) VALUES ('%s', '%s') """ %(col_1, col_2, key, value)
    # cursor.execute(sql)
    # db_conn.commit()
    # print("sdfsdf")

    # db_conn.close()
    ##################################

# 创建对象的基类:
Base = declarative_base()
# 定义User对象:
class User(Base):
    # 表的名字:
    __tablename__ = LANGUAGE_TABLE

    # 表的结构:
    id = Column(String(90), primary_key=True)
    lang = Column(String(500))


def getColNameList(tableName):
    db_conn = pymysql.connect(host=BASE_DB_HOST,
                    port=BASE_DB_PORT,
                    charset=BASE_DB_CHARSET,
                    user=BASE_DB_USER,
                    passwd=BASE_DB_PASSWORD,
                    db=BASE_DB_DATABASE)
    #默认游标
    # cursor = db_conn.cursor(cursor=pymysql.cursors.Cursor)
    # cursor = db_conn.cursor()
    #设置游标为字典类型
    cursor = db_conn.cursor(cursor=pymysql.cursors.DictCursor)

    # 获取列所有信息
    sql = """ show full columns from %s """ %(tableName)
    cursor.execute(sql)
    colInfo = cursor.fetchall()
    colNameList = []
    for row in colInfo:
        # print("Comment=", row['Comment'])
        if "[Language]" in row['Comment']:
            # print("colName=", row['Field'])
            colNameList.append(row['Field'])
    db_conn.close()
    return colNameList


def getTableInfoList():
    db_conn = pymysql.connect(host=BASE_DB_HOST,
                                    port=BASE_DB_PORT,
                                    charset=BASE_DB_CHARSET,
                                    user=BASE_DB_USER,
                                    passwd=BASE_DB_PASSWORD,
                                    db='information_schema',
                                    cursorclass=Cursor)
    cursor = db_conn.cursor()
    sql = """ SELECT TABLE_NAME, TABLE_COMMENT FROM TABLES WHERE table_schema = %s """
    cursor.execute(sql, BASE_DB_DATABASE)
    result = cursor.fetchall()
    tableInfoList = []
    for row in result:
        val = {'name':row[0], 'comment':row[1]}
        tableInfoList.append(val)
    return tableInfoList

def updataLanguageTable(key, value):
    col_1 = "id"
    col_2 = "lang"
    sql2 = """ REPLACE INTO `%s` (`%s`, `%s`) VALUES ('%s', '%s') """ %(LANGUAGE_TABLE, col_1, col_2, key, value)
    conn = engine.connect()
    conn.execute(sql2)


# 设置数据库
engine = create_engine('mysql+pymysql://chengxu:changit2017@192.168.199.156:3306/contra_base_en_test_cxf?charset=utf8', echo=False)
if engine is None:
    raise Exception("conn err")
DBSession = sessionmaker(bind=engine)
base_map = automap_base()
base_map.prepare(engine, reflect=True)

def writeToLanguage(tableInfo):
    tableName = tableInfo['name']
    tableComment = tableInfo['comment'] 
    if tableName == 'sensitive_word' or SKIPED_LANGUAGE in tableName or  SKIPED_FILE_TAG in tableComment:
        print 'skip table name ' + tableName
        return
    # print("tableName =" , tableName)


    session = DBSession()
    # table_class = base_map.classes[LANGUAGE_TABLE]  
    table_class = base_map.classes[tableName]
    # for tableName in base_map.classes:
    if table_class is None:
        raise Exception("table name err")
    records = session.query(table_class).all()
    if len(records) <= 0:
        print 'table is empty! name=' + tableName
        return
    # colName = "ability_name"
    colNameList = getColNameList(tableName)
    for colName in colNameList:
        for id in range(len(records)):
            record = records[id]
            key = tableName + "_" + colName + "_" + bytes(id+1)
            val = getattr(record, colName)
            if( tableName not in val) :
                # 写入language表
                updataLanguageTable(key, val)
                # 把key写入表 
                setattr(record, colName, key)
    session.commit()
    session.close()

def main():
    tableInfoList = getTableInfoList()
    for tableInfo in tableInfoList:
        writeToLanguage(tableInfo)


if __name__ == '__main__':
    # print sys.argv
    # if len(sys.argv) < 2:
    #     print "error arguments, need language tag!"
    #     sys.exit()
    # if sys.argv[1] == '0':
    #     BASE_DB_DATABASE = 'contra_base_test'
    # elif sys.argv[1] == '1':
    #     BASE_DB_DATABASE = 'contra_base_en_test'

	# getBaseData()
    # sqlalchemy_test()
    main()