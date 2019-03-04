# MovieWebApp

ASP.NET Core MVC によるWebアプリケーションです。  
機能として登録されているカテゴリ別に映画情報を表示します。  
映画情報は外部サイトの情報を利用しており、自動で更新されます。

## 概要

ASP.net core 2.2.1 を利用しています。

### カテゴリについて

表示する映画情報のカテゴリについては、以下のカテゴリになります。

- upcoming
  - 今後公開される映画
- popular
  - 知名度のある映画
- toprated
  - 人気のある映画

### 自動更新について
サイトで管理する映画の情報は以下のサイトのAPIを利用して1時間おきに自動更新します。  
https://www.themoviedb.org/  

映画情報を表示するにはアカウント登録と認証が必要です。  
アカウント登録については次の外部プロバイダーとの認証連携していますので、  
それぞのサイトで登録したのアカウントと連携することで、ログインすることができます。  

- Microsoft  
- Google  
- Twitter  

## サイトマップ

- /Movies  
  -  管理しているカテゴリすべての映画を一覧表示
- /Movies?category=upcoming
  -  今後公開される映画カテゴリを一覧表示
- /Movies?category=popular
  -  知名度のある映画カテゴリを一覧表示
- /Movies?category=toprated
  -  人気のある映画カテゴリを一覧表示

## Todo:  
- インポート時の代入を確認するテスト、正常系、代入するオブジェクト異常チェック、異常値のバリデーション


## 準備

ローカルで実行するには次の準備が必要です。

- TMDbLib

### TMDbLib

TMDbAPIを使うために、以下のC#のライブラリを利用しました。

TMDbLib 1.0.0  https://github.com/LordMike/TMDbLib  

パッケージインストール方法はNugetを使います。Packegemanagerから以下を実行します。

    PM> Install-Package TMDbLib  

# Azureでの公開について  

本WebアプリケーションはAzure WebServiceで公開しています。

https://moviewebapp20190303075609.azurewebsites.net/ 

このアプリケーションをVisualStudio2017からAzureに公開する手順の概要は以下となります。

## 1. プロジェクトで利用するDBの接続先をLocalSQL ServerからAzureのSQL Serverに変更する  

ローカル環境のプロジェクトからAzureのSQL Serverに接続します。

### 手順

- appsettings.jsonにConnectionStringsの中にAzureConnectionを追加  
- DefaultConnection から AzureConnection にする  
- PMCから Update-Database を実行してテーブルの作成を確認する  
- ローカルのWebサーバーを起動してAzure側のSQL ServerのDBへの参照と書き込みを確認する  
- 認証用のテーブルも空になるので、ユーザー登録とログインの確認を行う  

## 2. プロジェクトで利用するsecretManagerをVaultに移行する  

ローカルのseacrets.jsonで管理しているAPIキーなどのSeacret情報をVaultに移行します。 

### 手順

- VisualStudio2017のプロジェクトタイトルからVaultを有効にする  
- secrets.json に記載しているKeyValueをVaultに再設定する  
  - Valueの仕様上、Keyの値には:を使えないので注意する  

## 3. AzureのSQL Serverに移行後にAppServiceにPublish（発行）する  

### 手順

ビルドしたプロジェクトをAppServiceに発行します。

- VisualStudio2017でプロジェクトから発行を選択
- AppServerの設定
    - アプリ名、サブスクリプション、リソースグループ、ロケーションなどをなければ追加して選択
- 発行後、Webページを確認する

