select w.ID as WorkerID,  i.ID as InstructionID from Workers w
inner join Groups g on w.ID = g.ID
inner join InstructionGroups ig on ig.[GroupId] = g.ID
inner join Instructions i on i.ID = ig.InstructionId 
where w.IsSuspend = 0 and i.ID in (16,5,15)