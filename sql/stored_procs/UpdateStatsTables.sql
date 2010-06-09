USE [tweets]
GO
/****** Object:  StoredProcedure [dbo].[UpdateStatsTables]    Script Date: 06/09/2010 22:45:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Miles
-- Description:	Update stats tables for tf-idf calculation
--              tf_idf algorithm courtesy of http://en.wikipedia.org/wiki/Tf%E2%80%93idf
-- =============================================
ALTER PROCEDURE [dbo].[UpdateStatsTables]
AS
BEGIN
	SET NOCOUNT ON;

	begin transaction

	DELETE FROM stat_termcount
	DELETE FROM stat_user
	DELETE FROM stat_word

	INSERT INTO stat_termcount
	select screen_name, [text], count(id), null
	from word 
	group by screen_name, [text]
	order by [text], screen_name

	INSERT INTO stat_user
    select screen_name, count(id)
	from word 
	group by screen_name
	order by  screen_name

	INSERT INTO stat_word
    select [text], count(distinct(screen_name))
	from word 
	group by [text]
	order by  [text]

    UPDATE stat_termcount
	SET tf_idf = 
	 convert(float, [count])/convert(float,stat_user.word_count)*
	 log( convert(float, stat_usercount.total)/convert(float, stat_word.user_count)) 
	from stat_user, stat_word, stat_usercount
	where stat_user.screen_name=stat_termcount.screen_name
    and  stat_word.[text]=stat_termcount.[text]

	UPDATE stat_word 
	set avg_tf_idf=avg(tf_idf)
	FROM stat_termcount tc
	WHERE stat_word.[text] = tc.[text] 

	commit transaction


-- hopefully useful way to understand the above
--select
-- tc.screen_name,
-- tc.[text], 
-- tc.[count] term_count,
-- stat_user.word_count user_wordcount, 
-- stat_word.user_count usage_count,
-- stat_usercount.total total_doc,
-- log( convert(float, stat_usercount.total)/convert(float, stat_word.user_count)) idf,
-- convert(float, tc.[count])/convert(float,stat_user.word_count) tf,
-- convert(float, tc.[count])/convert(float,stat_user.word_count)*
-- log( convert(float, stat_usercount.total)/convert(float, stat_word.user_count)) tf_idf
--from stat_termcount tc 
--join stat_user
--     on stat_user.screen_name=tc.screen_name
--join stat_word
--     on stat_word.[text]=tc.[text],
--stat_usercount
--order by tf_idf desc

END
