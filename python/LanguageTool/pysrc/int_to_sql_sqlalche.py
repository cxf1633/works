#!/usr/bin/python
#-* coding:UTF-8 -*

# contra_base_test 是原始库 不可修改
# contra_base_en_test 是英文库，可以修改

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


# print sqlalchemy_utils.__version__

# 翻译过后的文件
dataPath = dname + "\\translateData\\"
# 获取翻译数据
def getDataFromTxt(fileName):
    if not os.path.exists(dataPath + fileName):
        print("=======================skip file name %s !========================" % fileName)
        return
    destFile = codecs.open(dataPath + fileName, 'r', 'utf-8')
    tr = csv.reader(destFile, delimiter=',',quotechar='', quoting=csv.QUOTE_NONE)

    # 列名
    titles = tr.next()
    colString = []
    for t in titles:
        print "t", t
        colString.append(t)
    # 内容
    dr = csv.reader(destFile, delimiter=',',quotechar='"', quoting=csv.QUOTE_ALL)
    dataString = []
    for row in dr:
        data = []
        for s in row:
            data.append(s)
        dataString.append(data)
    return colString,dataString

# 设置数据库
engine = create_engine('mysql+pymysql://chengxu:changit2017@192.168.199.156:3306/contra_base_en_test?charset=utf8', echo=False)
if engine is None:
    raise Exception("conn err")
DBSession = sessionmaker(bind=engine)
base_map = automap_base()
base_map.prepare(engine, reflect=True)

# 写入数据库
def wirteIntoSql(table_name, col_list, data_list):
    session = DBSession()
    table_class = base_map.classes[table_name]
    if table_class is None:
        raise Exception("table name err")
    records = session.query(table_class).all()
    # r = session.query(table_class).filter(getattr(table_class, "effect_id") == 700001, getattr(table_class, "lv") == 3)
    # r.update({'effect_name':'你好'})
    # session.commit()
    # print data_list[0]
    # return
    def compare(data, record):
        ret = 0
        for id in range(len(col_list)):
            if col_list[id].find("$") != -1:
                col = col_list[id].replace('$', '').strip()
                val = getattr(record, col)
                #print("val==", val, type(val))
                if type(val) is int:
                    val = str(val)
                if val != data[id]:
                    ret = -len(col_list)
                else:
                    ret = ret + 1
        return ret
    def replace(data, record):
        for id in range(len(col_list)):
            if col_list[id].find("$") == -1:
                print("col=", col_list[id])
                print("data=", data[id])
                setattr(record, col_list[id], data[id])
                session.add(record)
                
    for data in data_list:
        for record in records:
            if compare(data, record) > 0:
               replace(data, record)
    session.commit()
    session.close()

    # for data in data_list:
    #     ret = True
    #     for id in range(len(col_list)):
    #         if col_list[id].find("$") != -1:
    #             if getattr(record, col_list[id], None) != data[id]:
    #                 ret = False
    #     if ret:
    #         for id in range(len(col_list)):
    #             if col_list[id].find("$") == -1:
    #                 setattr(record, col_list[id], data[id])
    #         session.add(record)
    # session.commit()
    # session.close()

    # # records = session.query(table_class).filter(getattr(table_class, "effect_id") == 700001, getattr(table_class, "lv") == 3)
    # records = session.query(table_class).filter(getattr(table_class, "effect_id") == 700001, getattr(table_class, "lv") == 3)
    # for record in records:
    #     # for col in col_list:
    #     #     if col.find("$") != -1:
    #     #     else:
    #     # find = getattr(record, "intro", None)
    #     # if find is None:
    #     #     break
    #     setattr(record, "intro", "aaa")

    #     # print dir(record)
    #     # print record.__dict__
    #     session.add(record)
    # session.commit()
    # session.close()

# 主程序
def main():
    # 遍历翻译路径下的所有txt
    # files = os.listdir(dataPath)
    # for fileName in files:
        # print "fileName =", fileName
        fileName = "ability_score.txt"
        colList = []
        dataList = []
        colList,dataList = getDataFromTxt(fileName)

        tableName = fileName.replace('.txt', '').strip()
        print "tableName =", tableName
        wirteIntoSql(tableName, colList, dataList)



if __name__ == '__main__':
    main()

    # engine = create_engine('mysql+pymysql://chengxu:changit2017@192.168.199.156:3306/contra_base_en_cxf2')
    # if not database_exists(engine.url):  #=> False
    #     print("no database, create")
    #     create_database(engine.url)
    # # 绑定数据库
    # DBSession = sessionmaker(bind=engine)
    # table_name = "ability_score"
    # Base = automap_base()
    # Base.prepare(engine, reflect = True)

    # tableClass = Base.classes[table_name]
    # # 创建Session:
    # session = DBSession()
    # col = "sdf"
    # data = session.query(tableClass).filter(getattr(tableClass, col) == 7)
    # data.update({'ability_name': 'jisha'})
    # session.commit()
    # session.close()

