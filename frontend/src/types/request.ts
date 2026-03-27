export type RequestListItem = {
  id: string;
  title: string;
  description: string;
  createdBy: string;
  status: string;
  createdAt: string;
  updatedAt: string;
};

export type RequestComment = {
  author: string;
  comment: string;
  createdAt: string;
};

export type AuditLog = {
  action: string;
  performedBy: string;
  details: string;
  createdAt: string;
};

export type RequestDetail = RequestListItem & {
  camundaProcessInstanceKey?: string | null;
  comments: RequestComment[];
  auditLog: AuditLog[];
};
