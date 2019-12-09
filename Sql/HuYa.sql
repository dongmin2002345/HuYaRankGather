-- phpMyAdmin SQL Dump
-- version 4.8.3
-- https://www.phpmyadmin.net/
--
-- 主机： 10.53.54.151:13224
-- 生成日期： 2019-12-07 13:54:49
-- 服务器版本： 5.7.18-20170830-log
-- PHP 版本： 5.6.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 数据库： `HuYa`
--
CREATE DATABASE IF NOT EXISTS `HuYa` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `HuYa`;

-- --------------------------------------------------------

--
-- 表的结构 `RankInfo`
--

DROP TABLE IF EXISTS `RankInfo`;
CREATE TABLE `RankInfo` (
  `Id` int(11) NOT NULL,
  `UserId` bigint(20) NOT NULL COMMENT '用户Id',
  `UserNick` varchar(50) NOT NULL COMMENT '用户昵称',
  `RoomId` bigint(20) NOT NULL COMMENT '房间Id',
  `RoomOnline` int(11) NOT NULL COMMENT '在线观看人数',
  `RoomTitle` varchar(50) NOT NULL COMMENT '房间标题',
  `AppName` varchar(50) NOT NULL COMMENT '直播分类',
  `LogTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '记录时间'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- 转储表的索引
--

--
-- 表的索引 `RankInfo`
--
ALTER TABLE `RankInfo`
  ADD PRIMARY KEY (`Id`);

--
-- 在导出的表使用AUTO_INCREMENT
--

--
-- 使用表AUTO_INCREMENT `RankInfo`
--
ALTER TABLE `RankInfo`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
