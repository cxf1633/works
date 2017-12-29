@echo off
set /p id="Enter Language Tag(0:Chinese, 1:English): "
echo choose language id : %id%
python pysrc/outFromSql.py %id%
pause