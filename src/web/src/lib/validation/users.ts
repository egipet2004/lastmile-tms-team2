import { z } from "zod";
import { USER_ROLES } from "@/types/users";

export const userFormSchema = z
  .object({
    firstName: z.string().min(1, "First name is required").max(100),
    lastName: z.string().min(1, "Last name is required").max(100),
    email: z.string().min(1, "Email is required").email("Enter a valid email"),
    phone: z.string().max(20, "Phone must be 20 characters or fewer").optional(),
    role: z.enum(USER_ROLES),
    depotId: z.string().optional(),
    zoneId: z.string().optional(),
    isActive: z.boolean(),
  })
  .refine((value) => !value.zoneId || !!value.depotId, {
    path: ["zoneId"],
    message: "A depot must be selected when a zone is assigned.",
  });

export type UserFormSchema = z.infer<typeof userFormSchema>;
