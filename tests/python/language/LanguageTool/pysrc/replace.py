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
    
# 翻译好的文件放到这个文件夹下
dataPath = dname + "\\..\\translated\\"
# 旧的文件
oldFilePath = dname + "\\..\\oldFiles\\"

# 创建新的文件目录
def createFile():
    path = dname + "\\..\\newFiles\\"
    if not os.path.exists(path):
        os.mkdir(path)
    os.system("del /S/Q %s" % path)
    return path

def replaceTranslate(path, fileName):
    if not os.path.exists(oldFilePath + fileName):
        print("=======================skip file name %s !============================" % fileName)
        return

    # fileName = "skill_effect.txt"
    srcFile = codecs.open(dataPath + fileName, 'r', 'utf-8')
    s_reader = csv.reader(srcFile, delimiter=',',quotechar='', quoting=csv.QUOTE_NONE)
    # 表名字段
    srcTitle =s_reader.next()

    destFile = codecs.open(oldFilePath + fileName, 'r', 'utf-8')
    d_reader = csv.reader(destFile, delimiter=',',quotechar='', quoting=csv.QUOTE_NONE)
    # 表名字段
    destTitle = d_reader.next()

    # 获取dest字段位置
    def getPos(key):
        for i in range(len(destTitle)):
            title =destTitle[i]
            if title == key:
                return i
        return -1
    # 翻译过的关联字段
    s_keypos = []
    # 翻译过的翻译字段
    s_datapos = []
    # 没翻译的关联字段
    d_keypos = []
    # 没翻译的翻译字段
    d_datapos = []
    for id in range(len(srcTitle)):
        title =srcTitle[id]
        pos = title.find("$")
        if pos != -1:
            title = title.replace('$', '').strip()
            pos = getPos(title)
            if pos >=0:
                s_keypos.append(id)
                d_keypos.append(pos)
        else:
            pos = getPos(title)
            if pos >=0:
                s_datapos.append(id)
                d_datapos.append(pos)


    if len(d_keypos) <=0 or len(d_datapos) <=0:
        print("error!====>" + fileName +" does not contain key header or translation content!")
        return

    dw = csv.reader(destFile, delimiter=',',quotechar='"', quoting=csv.QUOTE_ALL)
    destData = {}
    for row in dw:
        mapKey = ''
        for index in d_keypos:
            mapKey = mapKey +"_"+row[index]
        destData[mapKey] = row
    
    # 替换
    newFile = codecs.open(path+fileName, 'w', 'utf-8')
    h = csv.writer(newFile,  delimiter = ',', quotechar='', quoting=csv.QUOTE_NONE)
    # 写入表名
    h.writerow(destTitle)
    w = csv.writer(newFile, delimiter = ',', quotechar = '"', quoting = csv.QUOTE_ALL)

    sw = csv.reader(srcFile, delimiter=',',quotechar='"', quoting=csv.QUOTE_ALL)
    sourceData = {}
    for row in sw:
        mapKey = ''
        for index in s_keypos:
            mapKey = mapKey +"_"+row[index]
        if destData.has_key(mapKey):
            for i in range(len(d_datapos)):
                destData[mapKey][d_datapos[i]] = row[s_datapos[i]]
        else:
            continue

    for row in destData.values():
        w.writerow(row)
            
    srcFile.close()
    destFile.close()
    newFile.close()

# replaceTranslate()

def main():
    path = createFile()
    files = os.listdir(dataPath)
    # print files
    for v in files:
        replaceTranslate(path, v)

main()
