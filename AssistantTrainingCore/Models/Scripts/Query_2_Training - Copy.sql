
--Nieprzeszkoleni, gdyz nie ma wpisu w Trainings lub jest wpis w Trainings, ale nie mam szkolenia
select w.ID as wID, i.ID as iID, w.LastName, i.Number as InstructionNumber, t.*
from Workers w
inner join GroupWorkers gw on gw.[WorkerId] = w.ID 
inner join Groups g on gw.[GroupId] = g.ID
inner join InstructionGroups ig on ig.[GroupId] = g.ID
inner join 
(
    select max_ins.ID,max_ins.Number from
    (
	   select i.ID,i.Version,i.Number from dbo.Instructions i
	   inner join 
	   (
		  select MAX(Version) as Ver,Number from dbo.Instructions
		  group by Number 
	   ) ig on i.[Version] = ig.Ver and i.Number = ig.Number
    ) max_ins 
) i on i.ID = ig.InstructionId 
left join Trainings t on t.WorkerId = w.ID  and i.ID = t.InstructionId
where   w.IsSuspend = 0  
	   --and i.ID IN (Select InstructionId from Trainings)
	   and (t.ID IS NULL or t.DateOfTraining = '1900-01-01 00:00:00.000')
order by i.ID
	   
	   
