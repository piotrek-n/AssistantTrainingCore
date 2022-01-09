--Old way
select distinct t.*, w.[IsSuspend] from TrainingGroups tg
inner join InstructionGroups ig on tg.InstructionId = ig.ID
inner join GroupWorkers gw on ig.GroupId  =  gw.GroupId 
inner join Trainings t on tg.TrainingNameId = t.TrainingNameId and gw.WorkerId = t.WorkerId
inner join Workers w on w.ID =  t.WorkerId 
where
tg.TrainingNameId =27  and w.[IsSuspend] = 0
--New way
select w.LastName as LastName,w.FirstMidName as FirstMidName,w.ID as WorkerID,
       t.TrainingNameId,t.DateOfTraining,tn.Number, t.InstructionId
from Trainings t 
inner join Workers w on w.ID =  t.WorkerId 
inner join TrainingNames tn on tn.ID = t.TrainingNameId
where DateOfTraining = '1900-01-01 00:00:00.000'

