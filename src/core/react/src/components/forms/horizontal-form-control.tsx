import {FormControl} from "../ui/form-control";
import {HStack} from "../ui/hstack";
import { ReactNode } from "react";

interface HorizontalFormControlProps {
  children?: ReactNode;
}

export default function HorizontalFormControl(
  {children}: HorizontalFormControlProps) {
  return (<FormControl>
    <HStack space="md">
      {children}
    </HStack>
  </FormControl>);
}