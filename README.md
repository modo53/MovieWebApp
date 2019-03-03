# MovieWebApp

ASP.NET Core MVC によるWebアプリケーションです。  
機能として登録されているカテゴリ別に映画情報を表示します。  
映画情報は外部サイトの情報を利用しており、自動で更新されます。

## 概要

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

- 
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

    
    
