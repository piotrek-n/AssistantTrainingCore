select w.ID as WorkerID,  i.ID as InstructionID, g.ID, t.* from Workers w
inner join GroupWorkers gw on gw.[WorkerId] = w.ID 
inner join Groups g on gw.[GroupId] = g.ID
inner join InstructionGroups ig on ig.[GroupId] = g.ID
inner join Instructions i on i.ID = ig.InstructionId 
left join Trainings t on t.WorkerId = w.ID  and i.ID = t.InstructionId
where   w.IsSuspend = 0  
	   and i.ID IN (16,17,18)
	   and t.ID IS NULL 

select w.ID as WorkerID,  i.ID as InstructionID, g.ID,
 (Select Top 1 [TrainingNames].Number from [dbo].[TrainingGroups]
  inner join  [dbo].[TrainingNames] on  [TrainingGroups].TrainingNameId = [TrainingNames].ID
  where [TrainingGroups].[InstructionId] = i.ID
  order by [TrainingGroups].[TimeOfCreation] desc
  )
from Workers w
inner join GroupWorkers gw on gw.[WorkerId] = w.ID 
inner join Groups g on gw.[GroupId] = g.ID
inner join InstructionGroups ig on ig.[GroupId] = g.ID
inner join Instructions i on i.ID = ig.InstructionId 
left join Trainings t on t.WorkerId = w.ID  and i.ID = t.InstructionId
where   w.IsSuspend = 0  
	   and i.ID IN (Select InstructionId from Trainings)
	   and t.ID IS NULL 

--from gw in db.GroupWorkers
--from ig in db.InstructionGroups
--join t in db.Trainings
--      on new { WorkerId = gw.Workers.ID, ig.Instructions.ID }
--  equals new { t.WorkerId, ID = t.InstructionId } into t_join
--from t in t_join.DefaultIfEmpty()
--where
--  gw.Workers.IsSuspend == false &&
--  (new int[] {16, 17, 18 }).Contains(ig.Instructions.ID) &&
--  t.ID == null
--select new {
--  WorkerID = (int?)gw.Workers.ID,
--  InstructionID = (int?)ig.Instructions.ID,
--  ID = (int?)ig.Groups.ID,
--  Column1 = (int?)t.ID,
--  InstructionId = (int?)t.InstructionId,
--  TrainingNameId = (int?)t.TrainingNameId,
--  WorkerId = (int?)t.WorkerId,
--  TimeOfCreation = (DateTime?)t.TimeOfCreation,
--  TimeOfModification = (DateTime?)t.TimeOfModification,
--  DateOfTraining = (DateTime?)t.DateOfTraining
--}



