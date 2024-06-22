import { Badge } from "@/components/ui/badge";
import { Bone } from "lucide-react";
const FilterBadge = () => {
  return (
    <Badge
      className="w-28 rounded-lg px-5 py-2 montserrat-regular flex items-center text-xs gap-x-2 bg-white shadow-lg lg:text-base lg:w-40"
      variant="outline"
    >
      <Bone size={16} />
      <span>Feeding</span>
    </Badge>
  );
};

export default FilterBadge;
