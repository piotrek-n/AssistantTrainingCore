select * from 
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
left join dbo.TrainingGroups tg
on tg.InstructionId = max_ins.ID
where tg.InstructionId IS NOT NULL
) ins_w_tr