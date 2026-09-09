import * as z from "zod";

export const getContactSchema = (t: (key: any) => string) =>
  z.object({
    name: z.string().min(2, t("form.nameError")).max(100),
    email: z.string().email(t("form.emailError")).max(255),
    department: z.string().optional(),
    phone: z.string().optional(),
    subject: z.string().min(3, t("form.subjectError")).max(200),
    message: z.string().min(10, t("form.messageError")).max(2000),
  });

export type ContactFormValues = z.infer<ReturnType<typeof getContactSchema>>;