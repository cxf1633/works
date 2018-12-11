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
newFilePath = dname + "\\translateData\\"
# 旧的文件
oldFilePath = dname + "\\oldFiles\\"

def outputData(data[]):
    # 对比结束 data 就是替换完成的数据
    newFile = codecs.open(path+fileName, 'w', 'utf-8')
    h = csv.writer(newFile,  delimiter = ',', quotechar='', quoting=csv.QUOTE_NONE)
    # 写入表名
    h.writerow(destTitle)
    w = csv.writer(newFile, delimiter = ',', quotechar = '"', quoting = csv.QUOTE_ALL)
    newFile.close()

# 单个文件
def replaceForEach(path, fileName):
    print("path===>>", path)
    print("fileName====>>>", fileName)
    if not oldFilePath+fileName:
        print("这个路径下没有这个txt")
        return

    # 翻译过的
    newFile = codecs.open(newFilePath + fileName, 'r', 'utf-8')
    #获取列名
    nreader = csv.reader(newFile, delimiter=',',quotechar='', quoting=csv.QUOTE_NONE)
    newDataTitle =nreader.next()
    #获取翻译数据
    nreader = csv.reader(newFile, delimiter=',',quotechar='"', quoting=csv.QUOTE_ALL)
    # 翻译文件数据
    newDataList = []
    for row in nreader:
        newDataList.append(row)     
    # 被替换的文件
    oldFile = codecs.open(oldFilePath + fileName, 'r', 'utf-8')
    oreader = csv.reader(oldFile, delimiter=',',quotechar='', quoting=csv.QUOTE_NONE)
    #获取列名
    oldDataTitle = oreader.next()
    #获取原数据
    oreader = csv.reader(oldFile, delimiter=',',quotechar='"', quoting=csv.QUOTE_ALL)
    # 当前版本数据
    oldDataList = []
    for row in oreader:
        oldDataList.append(row)


     # 根据翻译内容，到原文件中对比
    for data in newDataList:
        if isMatch(data[key]):
            replace(data)

    srcFile.close()
    destFile.close()
def main():
    # 输出路径
    path = dname + "\\outputFiles\\"
    if not os.path.exists(path):
        os.mkdir(path)
    os.system("del /S/Q %s" % path)

    # 根据翻译文件列表进行替换
    files = os.listdir(newFilePath)
    # print files
    for v in files:
        replaceForEach(path, v)


# main()

def test():
    ss = [1,2,3]
    for d in ss:
        print("d===>", d)
test()
