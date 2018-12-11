#!/usr/bin/python
#-* coding:UTF-8 -*

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

# 初始化数据库连接:
engine = create_engine('mysql+pymysql://chengxu:changit2017@192.168.199.156:3306/contra_base_en_cxf')
# print(engine)
# 创建DBSession类型:
DBSession = sessionmaker(bind=engine)

# 创建对象的基类:
Base = declarative_base()

# 定义Table_Box对象:
class Table_Box(Base):
    # 表的名字:
    __tablename__ = 'box'

    # 表的结构:
    box_id = Column(Integer, primary_key=True)
    name = Column(String(100))

# 查询表
def queryTable(table_name, id):
    # 创建Session: 
    session = DBSession()
    # 创建Query查询，filter是where条件，最后调用one()返回唯一行，如果调用all()则返回所有行:
    user = session.query(table_name).filter(table_name.box_id==id).one()
    # user = session.query(User).all()
    # 打印类型和对象的name属性:
    print 'type:', type(user)
    print 'name:', user.name

#更新表 
def updateTable(table_name):
    # 创建Session:
    session = DBSession()
    Base = automap_base()
    Base.prepare(engine, reflect = True)  

    # user_alias = aliased(Base.classes.box, name='user_alias')
    # 查询操作  
    result = session.query(Base.classes[table_name]).all() 
    for data in result:
        print data.id
        print data.type

    # tableClass = Base.classes[table_name]
    # one = session.query(tableClass).filter(tableClass.id==id).one()
    # print(one.ability_name)

    # 修改 
    # data = session.query(tableClass).filter(tableClass.id == id)
    # print data.count()
    # if data.count() > 0:
    #     print("data change")
    #     data.update({'ability_name': 'cxf test333'})
    # else:
    #     print("query error:" + id)
    
    session.commit()
    session.close()

def autoCreateTable():
    table_name = "ability_score"
    Base = automap_base()
    Base.prepare(engine, reflect = True)
    tableClass = Base.classes[table_name]
    # 创建Session:
    session = DBSession()
    data = session.query(tableClass).filter(tableClass.id == 7)
    data.update({'ability_name': 'cxf test333'})
    session.commit()
    session.close()
# # rudimentary relationships are produced
#     session.add(Address(email_address="foo@bar.com", user=User(name="foo")))
#     session.commit()
# 主程序
def main():    
    # print "main"
    # updateTable("ability_score")
    
    autoCreateTable()

main()