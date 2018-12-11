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


# 翻译过后的文件
dataPath = dname + "\\..\\newFiles\\"
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
        
    session.query(table_class).delete()

    for data in data_list:
        obj = table_class()
        for n in range(len(col_list)):
            setattr(obj, col_list[n], data[n])
        session.add(obj)

    session.commit()
    session.close()

# 主程序
def main():
    # 遍历翻译路径下的所有txt
    files = os.listdir(dataPath)
    for fileName in files:
        print "fileName =", fileName
        # fileName = "ability_score.txt"
        colList = []
        dataList = []
        colList,dataList = getDataFromTxt(fileName)

        tableName = fileName.replace('.txt', '').strip()
        # print "tableName =", tableName
        wirteIntoSql(tableName, colList, dataList)



if __name__ == '__main__':
    main()


