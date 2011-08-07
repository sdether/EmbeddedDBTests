EmbeddedDBTests
===============

License
=======
Copyright (C) 2011 Arne F. Claassen

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

Requirements
============

These examples depend on a Stackoverflow data dump schema compatible with the 042010 dump

Contents
========
BinaryDataTests
---------------
Post_Xml_to_Bin and User_Xml_to_Bin convert Stackoverflow xml to protobuf binary streams. These two are pre-requisites for pretty much everything else

FirkinDataTests
---------------
Converting the protobuf streams to firkin DBs and some examples using Firkin. Firkin is abstracted via FirkinDataSource which returns a FirkinDictionary to let you treat the database as a dictionary

BerkelyDBTests
--------------
Converting the protobuf streams to berkeley DBs and some examples using BerkeleyDB. BerkeleyDB is abstracted via BerkeleyDBDataSource which creates BDBWRApper to let you treat the database as a dictionary

LuceneTests
-----------
Converting from protobuf streams to lucene index and some examples using Lucene. LuceneDataSource abstracts the Lucene index as LuceneDB to let you treat the index as a dictionary
