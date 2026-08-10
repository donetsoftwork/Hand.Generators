# .NET源生成器之Attribute解析和生成

## 前言
### 1. Attribute很重要
>* 作为元数据描述
>* 作为编译参数,影响编译结果
>* 运行是通过反射读取,影响运行结果
>* 作为源生成器触发条件或参数,影响生成的代码

### 2. SyntaxTree和ISymbol
>* 本文探讨基于SyntaxTree和ISymbol解析Attribute
>* 基于SyntaxTree生成Attribute
>* 当然string拼接也可以实现,本文致力于使用Roslyn语法安全的生成代码

## 一、Attribute解析
### 1. 从SyntaxTree解析Attribute

### 2. 从ISymbol解析

## 二、生成Attribute

