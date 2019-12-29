# MathLibTest
Test of different C++ mathematical libraries:

* [Armadillo](https://en.wikipedia.org/wiki/Armadillo_(C%2B%2B_library))
* BLASDirect (native compilation of [BLAS](https://en.wikipedia.org/wiki/Basic_Linear_Algebra_Subprograms) for Microsoft Visual Studio)
* [Boost](https://en.wikipedia.org/wiki/Boost_(C%2B%2B_libraries))
* [Eigen](https://en.wikipedia.org/wiki/Eigen_(C%2B%2B_library))
* [OpenBLAS](https://en.wikipedia.org/wiki/OpenBLAS)
* [OpenCV](https://en.wikipedia.org/wiki/OpenCV)

**Language: C++ / C#**

**Start: 2015**

## Why
I wanted to compare different C++ mathematical libraries. I created _MathLibCompare_ to compare performance of the libraries while doing a multiplication of two 1024 square matrices.

## Notes on compiling BLAS to dll (BLASDirect) for Microsoft Visual Studio (MSVS)
I started from notes found online that I then changed to:

1) install gfortran-windows-20140629.exe
2) open a command prompt and go to _BLAS-3.5.0\out_
3) _gfortran -O2 -c ..\src\*.f_
4) _gfortran -shared *.o -static-libgfortran -o libblas.dll_
5) open a 'MSVS command prompt' and go to _BLAS-3.5.0\out_
6) _dumpbin /exports libblas.dll > libblas.def_
7) open _libblas.def_ with notepad and change the file to something like:
```
EXPORTS
_gfortran_adjustl
_gfortran_adjustl_char4
_gfortran_adjustr
_gfortran_adjustr_char4
_gfortran_arandom_r10
_gfortran_arandom_r16
...
```
8) _lib /def:libblas.def /OUT:libblas.lib_
9) copy _libblas.dll_ and _libblas.lib_ to the _lib_ folder of the MSVS project
10) add _libblas.lib_ in the MSVS menu _Project_->_Properties_->_Linker_->_Input_->_Additional Dependencies_ 

## Results

Performance results multipling two 1024x1024 matrices:

Name                     | milliseconds
-------------------------|-----------------
Simple in C# (optimized) | 38895
Simple in C++            | 15258
Armadillo                | 1198 (*)
BLASDirect               | 1439 (*)
Boost                    | 1460
Eigen                    | 893 (*)
OpenBLAS                 | 316
OpenCV                   | 912

_(*) these seems to have to compute a matrix a bit different from the other ones._
----- End