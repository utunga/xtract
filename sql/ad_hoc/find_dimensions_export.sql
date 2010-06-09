--SELECT top 1000 tc.[text],avg( tf_idf) tf_idf
--FROM stat_termcount tc,  stat_word
--WHERE
-- stat_word.[text] = tc.[text] 
--and stat_word.user_count>2
--group by tc.[text]
--ORDER BY avg( tf_idf) DESC
--
----0.000234771575501096 = 1000 dimensions

SELECT twuser.twitter_user_id, tc.screen_name, tc.[text],  [count], avg_count, stat_word.user_count user_co_occurences, tf_idf, avg_tf_idf
FROM stat_termcount tc
JOIN stat_word on stat_word.[text] = tc.[text] 
JOIN twuser on twuser.screen_name=tc.screen_name
WHERE 
 stat_word.included=1
ORDER BY screen_name, [count] desc, [text], stat_word.avg_tf_idf  DESC
